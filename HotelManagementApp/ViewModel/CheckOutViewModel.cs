using HotelManagementApp.Model;
using HotelManagementApp.View;
using iTextSharp.text.pdf;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HotelManagementApp.ViewModel
{
    public class CheckOutViewModel : BaseViewModel
    {
        private ObservableCollection<RoomsReservation> _FilteredList;
        public ObservableCollection<RoomsReservation> FilteredList { get => _FilteredList; set { _FilteredList = value; OnPropertyChanged(); } }
        private string _SearchString;
        public string SearchString { get => _SearchString; set { _SearchString = value; LoadFilteredList(); OnPropertyChanged(); } }
        private ObservableCollection<RoomsReservation> _roomRevsCheckOut;
        public ObservableCollection<RoomsReservation> RoomRevsCheckOut { get => _roomRevsCheckOut; set { _roomRevsCheckOut = value; OnPropertyChanged(); } }
        private int? _Nights;
        public int? Nights { get => _Nights; set { _Nights = value; OnPropertyChanged(); } }
        private string _checkOutDate;
        public string CheckOutDate { get => _checkOutDate; set { _checkOutDate = value; OnPropertyChanged(); } }
        private string _checkInDate;
        public string CheckInDate { get => _checkInDate; set { _checkInDate = value; OnPropertyChanged(); } }
        private decimal? _FnSTotal;
        public decimal? FnSTotal { get => _FnSTotal; set { _FnSTotal = value; OnPropertyChanged(); } }
        private decimal? _RoomsTotal;
        public decimal? RoomsTotal { get => _RoomsTotal; set { _RoomsTotal = value; OnPropertyChanged(); } }
        private bool _billConfirmed = false;
        public bool BillConfirmed { get => _billConfirmed; set { _billConfirmed = value; OnPropertyChanged(); } }
        private PaymentWindow paymentWindow;
        public PaymentWindow PaymentWindow { get => paymentWindow; set { paymentWindow = value; OnPropertyChanged(); } }
        private RoomsReservation _SelectedReservation;
        public RoomsReservation SelectedReservation
        {
            get => _SelectedReservation;
            set
            {
                _SelectedReservation = value;
                if (value != null)
                {
                    SelectedBill = Global.BillsList.Where(x => x.ID == _SelectedReservation.IDBillDetail).FirstOrDefault();
                    Nights = (int)_SelectedReservation.CheckOutTime.Value.Subtract(_SelectedReservation.CheckInTime.Value).TotalDays;
                }
                OnPropertyChanged();
            }
        }

        private BillDetail _SelectedBill;
        public BillDetail SelectedBill
        {
            get => _SelectedBill;
            set
            {
                _SelectedBill = value;

                OnPropertyChanged();
            }
        }
        private BillDetail _bill;
        public BillDetail Bill
        {
            get => _bill;
            set
            {
                _bill = value;
                OnPropertyChanged();
            }
        }

        public ICommand CheckOutCommand { get; set; }
        public ICommand ConfirmBillBtn { get; set; }
        public ICommand ExportPdfBtn { get; set; }
        public CheckOutViewModel()
        {
            LoadFilteredList();
            CheckOutCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedBill == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                Bill = DataProvider.Instance.DB.BillDetails.Where(x => x.ID == SelectedBill.ID).FirstOrDefault();
                PaymentWindow = new PaymentWindow();
                Bill.BillDate = DateTime.Now;
                CheckInDate = ((DateTime)Bill.RoomsReservations.FirstOrDefault().CheckInTime).ToString("G");
                CheckOutDate = ((DateTime)Bill.RoomsReservations.FirstOrDefault().CheckOutTime).ToString("G");
                RoomsTotal = Bill.RoomsReservations.Select(x => x.Room.RoomType.Price * Nights).Sum();
                FnSTotal = Bill.Orders.Select(x => x.TotalPrice).Sum();
                PaymentWindow.ShowDialog();
            });
            ConfirmBillBtn = new RelayCommand<object>((p) => true, (p) =>
            {
                var SBill = Bill;
                Bill.Status = "Completed";
                foreach (var item in Bill.RoomsReservations)
                {
                    Global.RoomsList.Remove(item.Room);
                    item.Room.Status = "Available";
                    Global.RoomsList.Add(item.Room);
                    Global.OnGoingReservationsList.Remove(item);
                }
                Global.BillsList.Remove(SBill);
                Global.BillsList.Add(Bill);
                DataProvider.Instance.DB.SaveChanges();
                SelectedReservation = null;
                SelectedBill = null;
                SearchString = null;
                PaymentWindow.Close();
                LoadFilteredList();
            });
            ExportPdfBtn = new RelayCommand<object>((p) => true, (p) =>
            {
                string destinationDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                var imageFile = destinationDirectory + $"\\Bills\\bill{Bill.ID}.png";

                RenderTargetBitmap render = new RenderTargetBitmap((int)this.PaymentWindow.ActualWidth, (int)this.PaymentWindow.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                render.Render(this.PaymentWindow);

                MemoryStream cropStream = new MemoryStream();
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(render));
                encoder.Save(cropStream);
                Bitmap cropImage = new Bitmap(cropStream);
                RectangleF crop = new RectangleF(0, 30, (float)PaymentWindow.ActualWidth, (float)PaymentWindow.ActualHeight - 80);
                cropImage = cropImage.Clone(crop, cropImage.PixelFormat);
                cropImage.Save(imageFile);

                string pdfFile = destinationDirectory + $"\\Bills\\bill{Bill.ID}.pdf";

                using (var ms = new MemoryStream())
                {
                    var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER.Rotate(), 0, 0, 0, 0);
                    PdfWriter.GetInstance(document, new FileStream(pdfFile, FileMode.Create));
                    iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms).SetFullCompression();
                    document.Open();

                    FileStream fs = new FileStream(imageFile, FileMode.Open);
                    var image = iTextSharp.text.Image.GetInstance(fs);
                    image.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
                    image.Alignment = iTextSharp.text.Image.ALIGN_CENTER;

                    document.Add(image);
                    document.Close();
                    fs.Dispose();
                }
                System.IO.File.Delete(imageFile);
                Process.Start("explorer.exe", pdfFile);
            });
        }

        private void LoadFilteredList()
        {
            var list = new ObservableCollection<RoomsReservation>();
            if (string.IsNullOrEmpty(SearchString))
            {
                list = Global.OnGoingReservationsList;
            }
            else
            {
                foreach (var item in Global.OnGoingReservationsList)
                {
                    if (item.Room.RoomNum.StartsWith(SearchString))
                    {
                        list.Add(item);
                    }
                }
            }
            FilteredList = list;
        }
    }
}
