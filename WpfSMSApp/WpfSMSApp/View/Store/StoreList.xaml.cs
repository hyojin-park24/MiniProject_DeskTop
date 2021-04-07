using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace WpfSMSApp.View.Store
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StoreList : Page
    {
        public StoreList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //Store 테이블에서 데이터 읽어와야함
                List<Model.Store> stores = new List<Model.Store>();
                List<Model.StockStore> stockStores = new List<Model.StockStore>();
                stores = Logic.DataAccess.GetStores();
              

                // Store 데이터를 stockStores로 복사한 쿼리문
                foreach (Model.Store item in stores)
                {
                    var store = new Model.StockStore()
                    {
                        StoreID = item.StoreID,
                        StoreName = item.StoreName,
                        StoreLocation = item.StoreLocation,
                        ItemStatus = item.ItemStatus,
                        TagID = item.TagID,
                        BarcodeID = item.BarcodeID,
                        StockQuantity = 0
                    };
                    
                    store.StockQuantity = Logic.DataAccess.GetStocks().Where(t => t.StoreID.Equals(store.StoreID)).Count();
                    stockStores.Add(store);
                }

                this.DataContext = stockStores;
            }
            catch (Exception ex)
            {

                Commons.LOGGER.Error($"예외발생 StoreList Loaded : {ex}");
                throw ex;
            }
        }

        private void BtnEidtMyAccount_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new EditAccount()); // 계정정보 수정 화면으로 변경
        }


        private void BtnAddStore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new AddStore());
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnAddStore_Click : {ex}");
            }
        }

        private void BtnEditStore_Click(object sender, RoutedEventArgs e)
        {
            if (GrdData.SelectedItem == null)
            {
                Commons.ShowMessageAsync("창고 수정", "수정할 창고를 선택하세요");
                return;
            }    
            try
            {
                var storeId = (GrdData.SelectedItem as Model.Store).StoreID;
                NavigationService.Navigate(new EditStore(storeId));
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnEditStore_Click : {ex}");
            }
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Excel File (*.xlsx)|*.xlsx"; //엑셀 확장자
            dialog.FileName = "";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    IWorkbook workbook = new XSSFWorkbook(); // xls(이전버전용)
                    ISheet sheet = workbook.CreateSheet("Sheet1"); // 이름변경 가능
                                                                   // 헤더 row 
                    IRow rowHeader = sheet.CreateRow(0);
                    ICell cell = rowHeader.CreateCell(0);
                    cell.SetCellValue("순번");
                    cell = rowHeader.CreateCell(1);
                    cell.SetCellValue("창고명");
                    cell = rowHeader.CreateCell(2);
                    cell.SetCellValue("창고 위치");
                    cell = rowHeader.CreateCell(3);
                    cell.SetCellValue("재고수");

                    for (int i = 0; i < GrdData.Items.Count; i++)
                    {
                        IRow row = sheet.CreateRow(i + 1);
                        var stockStore = GrdData.Items[i] as Model.StockStore;
                        ICell dataCell = row.CreateCell(0);
                        dataCell.SetCellValue(stockStore.StoreID);
                        dataCell = row.CreateCell(1);
                        dataCell.SetCellValue(stockStore.StoreName);
                        dataCell = row.CreateCell(2);
                        dataCell.SetCellValue(stockStore.StoreLocation);
                        dataCell = row.CreateCell(3);
                        dataCell.SetCellValue(stockStore.StockQuantity);

                    }

                    // 파일 저장 
                    using (var fs = new FileStream(dialog.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        workbook.Write(fs);
                    }

                    Commons.ShowMessageAsync("엑셀 저장", "엑셀 Export 성공!");
                }
                catch (Exception ex)
                {
                    Commons.ShowMessageAsync("예외",$"예외발생:{ex}");
                }
            }
         }
    }
}
