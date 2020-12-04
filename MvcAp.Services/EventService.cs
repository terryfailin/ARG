using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Helpers;
using Microsoft.Security.Application;
using MvcAp.Common;
using MvcAp.Common.Helper;
using MvcAp.Models;
using Newtonsoft.Json;
using MvcAp.Models.ViewModels;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using ZXing;

namespace MvcAp.Services
{
    public class EventService : GenericService
    {
        
        public Event Create(EventViewModel data, HttpPostedFileBase titlefile)
        {
            bool isNeedPush = false;
            Event ev = null;
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {                    
                    int userid = BackendUserHelper.CurrentUserLiteData.ID;
                    
                    ev = new Event()
                    {
                        Title = data.Title,
                        Name = data.Name,
                        EventStart = data.EventStart,
                        EventEnd = data.EventEnd,
                        SignStart = data.SignStart,
                        SignEnd = data.SignEnd,
                        MasterEmail = data.MasterEmail,
                        UserId = data.UserId,
                        IsPwLimited = data.IsPwLimited,
                        IsNumLimited = data.IsNumLimited,
                        IsOpen = true,
                        LimitedNum = data.LimitedNum,
                        LimitedPw = data.LimitedPw,
                        IsChangeViewNum = data.IsChangeViewNum,
                        ViewNum = data.ViewNum,
                        Content = data.Content,
                        EmailContent = data.EmailContent,
                        CfmEmailContent = data.CfmEmailContent,
                        Agreement = data.Agreement,
                        DepId=data.DepId,
                        Address=data.Address,
                        QRcodeMode = data.QRcodeMode,
                        QRCodePassword = data.QRCodePassword

                    };
                    if (String.IsNullOrEmpty(ev.Code))
                    {
                        ev.Code = ImageHelper.GenerateId();
                    }
                    context.Event.Add(ev);
                    context.SaveChanges();
                    if (ev.IsOpen)
                    {
                        isNeedPush = true;
                    }
                    #region 圖片處理
                    //string folderName = ConfigurationManager.AppSettings["ImgArticle"];

                    //string moveToFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName, ev.ID.ToString());

                    //DirectoryInfo diMoveTo = new DirectoryInfo(moveToFolder);

                    //if (!diMoveTo.Exists)
                    //{
                    //    diMoveTo.Create();
                    //}
                    //List<string> tmpList = titlefile.FileName.Split('.').ToList();
                    //if (tmpList.Count != 2)
                    //{
                    //    throw new ExpectedException("Image Format Invalid");
                    //}

                    //string cropFilename = Guid.NewGuid().ToString();
                    //string filename = cropFilename + "." + tmpList[1];
                    //cropFilename = cropFilename + "_crop." + tmpList[1];
                    //string savePath = Path.Combine(moveToFolder, filename);
                    //string cropPath = Path.Combine(moveToFolder, cropFilename);
                    //titlefile.SaveAs(savePath);

                    //using (var img = Image.FromFile(savePath))
                    //{
                    //    string extensionName = string.Empty;
                    //    if (img.RawFormat.Equals(ImageFormat.Jpeg))
                    //    {
                    //        extensionName = ".jpg";
                    //    }
                    //    else if (img.RawFormat.Equals(ImageFormat.Gif))
                    //    {
                    //        extensionName = ".gif";
                    //    }
                    //    else if (img.RawFormat.Equals(ImageFormat.Png))
                    //    {
                    //        extensionName = ".png";
                    //    }
                    //    else if (img.RawFormat.Equals(ImageFormat.Bmp))
                    //    {
                    //        extensionName = ".bmp";
                    //    }
                    //    else
                    //    {
                    //        throw new ExpectedException("圖片格式異常");
                    //    }
                    //    var cImage = ImageHelper.CropImageFile(img, data.x1, data.y1, data.w, data.h);
                    //    cImage.Save(cropPath);
                    //}
                    //string filepath = string.Format("/{0}/{1}", ev.ID.ToString(), cropFilename);
                    //ev.TitleImgPath = filepath;
                    //context.SaveChanges();
                    #endregion                    
                }
                scope.Complete();
            }
            return ev;
        }

        public string GetDefaultAgree()
        {
            string result = "";
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    var agree = context.Agreement.FirstOrDefault(p => (bool)p.IsPublish);
                    if (agree!=null)
                    {
                        result = agree.Contents;
                    }
                }
            }
            return result;
        }

        public int GetDepartmentId()
        {
            int depid = 0;
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    var currentUser = BackendUserHelper.CurrentUserLiteData;
                    depid = context.SystemUser.FirstOrDefault(p => p.ID == currentUser.ID).DepId;                    
                }
            }
            return depid;
        }

        public void Update(EventViewModel data, HttpPostedFileBase titlefile)
        {
            bool isNeedPush = false;
            Event vo = null;
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    int userid = BackendUserHelper.CurrentUserLiteData.ID;

                    vo = context.Event.FirstOrDefault(p => p.ID == data.ID);
                    string eventCode = ImageHelper.GenerateId();
                    if (vo == null)
                    {
                        throw new Exception("Event ID Not Exist");
                    }
                    if (String.IsNullOrEmpty(vo.Code))
                    {
                        vo.Code = ImageHelper.GenerateId();
                    }
                    vo.Title = data.Title;
                    vo.Name = data.Name;
                    vo.EventStart = data.EventStart;
                    vo.EventEnd = data.EventEnd;
                    vo.SignStart = data.SignStart;
                    vo.SignEnd = data.SignEnd;
                    vo.MasterEmail = data.MasterEmail;
                    vo.UserId = data.UserId;
                    vo.IsPwLimited = data.IsPwLimited;
                    vo.IsNumLimited = data.IsNumLimited;
                    vo.IsOpen = data.IsOpen;
                    vo.LimitedNum = data.LimitedNum;
                    vo.LimitedPw = data.LimitedPw;
                    vo.IsChangeViewNum = data.IsChangeViewNum;
                    vo.ViewNum = data.ViewNum;
                    vo.Content = data.Content;
                    vo.EmailContent = data.EmailContent;
                    vo.CfmEmailContent = data.CfmEmailContent;
                    vo.Agreement = data.Agreement;
                    vo.DepId = data.DepId;
                    vo.Address = data.Address;
                    vo.QRcodeMode = data.QRcodeMode;
                    vo.QRCodePassword = data.QRCodePassword;
                    
                    context.SaveChanges();

                }
                scope.Complete();
            }
        }

        public Event CopyEvent(Event data)
        {
            Event ev = null;
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    #region Copy Event 
                    ev = new Event()
                    {
                        Title = data.Title+"_"+DateTime.Now.ToString("yyMMddHHmm"),
                        Name = data.Name + "_" + DateTime.Now.ToString("yyMMddHHmm"),
                        EventStart = data.EventStart,
                        EventEnd = data.EventEnd,
                        SignStart = data.SignStart,
                        SignEnd = data.SignEnd,
                        MasterEmail = data.MasterEmail,
                        UserId = data.UserId,
                        IsPwLimited = data.IsPwLimited,
                        IsNumLimited = data.IsNumLimited,
                        IsOpen = true,
                        LimitedNum = data.LimitedNum,
                        LimitedPw = data.LimitedPw,
                        IsChangeViewNum = data.IsChangeViewNum,
                        ViewNum = data.ViewNum,
                        Content = data.Content,
                        EmailContent = data.EmailContent,
                        CfmEmailContent = data.CfmEmailContent,
                        Agreement = data.Agreement,
                        DepId = data.DepId,
                        Address = data.Address,
                        QRcodeMode = data.QRcodeMode,
                        QRCodePassword = data.QRCodePassword

                    };
                    if (String.IsNullOrEmpty(ev.Code))
                    {
                        ev.Code = ImageHelper.GenerateId();
                    }
                    context.Event.Add(ev);
                    context.SaveChanges();
                    #endregion

                    #region Copy EventRegist

                    if (data.EventRegist!=null)
                    {
                        foreach (EventSign evSign in data.EventSign.ToList())
                        {
                            var evs = new EventSign()
                            {
                                EventId = ev.ID,
                                TypeId = evSign.TypeId,
                                Title = evSign.Title,
                                MinLen = evSign.MinLen,
                                MaxLen = evSign.MaxLen,
                                IsRequired = evSign.IsRequired == null ? false : evSign.IsRequired,
                                OrderNum = evSign.OrderNum,
                                UseType = evSign.UseType == null ? 0 : evSign.UseType
                            };
                            context.EventSign.Add(evs);
                            context.SaveChanges();

                            var evDrops = context.EventDropList.Where(p => p.EventSignId == evSign.ID).ToList();
                            if (evDrops!=null && evDrops.Count>0)
                            {
                                foreach (var evDrop in evDrops)
                                {
                                    var evd = new EventDropList()
                                    {
                                        EventSignId = evs.ID,
                                        RowValue = evDrop.RowValue,
                                        IsDefault = evDrop.IsDefault==null? false:evDrop.IsDefault
                                    };
                                    context.EventDropList.Add(evd);
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                    

                    #endregion
                }
                scope.Complete();
            }
            return ev;
        }

        public void Delete(List<int>  data)
        {
            if (data != null)
            {
                using (var scope = new TransactionScope())
                {
                    using (var context = new Entities())
                    {
                        var deleteEventList = context.Event.Where(p => data.Contains(p.ID)).ToList();
                        var deleteEventRegList = context.EventRegist.Where(p => data.Contains(p.EventId)).ToList();
                        var deleteEventSignList =context.EventSign.Where(p => data.Contains(p.EventId)).ToList();
                        List < int > deleteEventSignIds =deleteEventSignList.Select(p => p.ID).ToList();                            
                        var deleteEventDropList =context.EventDropList.Where(p => deleteEventSignIds.Contains(p.EventSignId)).ToList();

                        context.EventDropList.RemoveRange(deleteEventDropList);
                        context.EventSign.RemoveRange(deleteEventSignList);
                        context.EventRegist.RemoveRange(deleteEventRegList);
                        context.Event.RemoveRange(deleteEventList);
                                                
                        context.SaveChanges();
                    }
                    scope.Complete();
                }
            }
        }
        public EventSign CreateEventSign(EventSignViewModel data)
        {
            EventSign es = null;
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    es = new EventSign()
                    {
                        EventId = data.EventId,
                        TypeId = data.TypeId,
                        Title = data.Title,
                        MinLen = data.MinLen,
                        MaxLen = data.MaxLen,
                        IsRequired = data.IsRequired,
                        UseType = data.UseType,
                    };
                    if (!String.IsNullOrEmpty(data.TagOpts))
                    {
                        string[] opts = data.TagOpts.Split(',');
                        foreach (var opt in opts)
                        {
                            EventDropList eopt = new EventDropList();
                            eopt.EventSignId = es.ID;
                            eopt.RowValue = opt;
                            context.EventDropList.Add(eopt);
                        }
                    }

                    context.EventSign.Add(es);
                    context.SaveChanges();
                }
                scope.Complete();
            }
            return es;
        }
        public void UpdateEventSign(EventSignViewModel data)
        {
            EventSign es = null;
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    es = context.EventSign.FirstOrDefault(p => p.ID == data.ID);
                    es.EventId = data.EventId;
                    es.TypeId = data.TypeId;
                    es.Title = data.Title;
                    es.MinLen = data.MinLen;
                    es.MaxLen = data.MaxLen;
                    es.IsRequired = data.IsRequired;
                    es.UseType = data.UseType;
                    var delDropList = context.EventDropList.Where(p => p.EventSignId == data.ID);
                    if (!String.IsNullOrEmpty(data.TagOpts))
                    {
                        context.EventDropList.RemoveRange(delDropList);
                        string[] opts = data.TagOpts.Split(',');
                        foreach (var opt in opts)
                        {
                            EventDropList eopt = new EventDropList();
                            eopt.EventSignId = es.ID;
                            eopt.RowValue = opt;
                            context.EventDropList.Add(eopt);
                        }
                    }
                        
                    context.SaveChanges();
                }
                scope.Complete();
            }            
        }

        public void DeleteEventSign(int esid)
        {
            if (esid > 0)
            {
                using (var scope = new TransactionScope())
                {
                    using (var context = new Entities())
                    {
                        EventSign es = context.EventSign.FirstOrDefault(p => p.ID == esid);
                        if (es!=null)
                        {
                            var removeList=context.EventDropList.Where(p => p.EventSignId == esid).ToList();
                            context.EventDropList.RemoveRange(removeList);                            
                            context.EventSign.Remove(es);
                            context.SaveChanges();
                        }
                    }
                    scope.Complete();
                }
            }
        }

        public List<SystemUser> CallAvailableUsers()
        {
            List<string> userRoles = BackendUserHelper.CurrentUserLiteData.RoleList;
            List<SystemUser> users;
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    if (userRoles.Contains("System Admin"))
                    {
                        users = context.SystemUser.Where(p => p.SystemUserRoles.Any(r => r.RoleId >= 3)).ToList();
                    }
                    else if (userRoles.Contains("Director"))
                    {
                        var currentUser = BackendUserHelper.CurrentUserLiteData;
                        int depid = context.SystemUser.FirstOrDefault(p => p.ID == currentUser.ID).DepId;
                       
                        users =
                            context.SystemUser.Where(p => p.DepId==depid)
                                .ToList();
                    }
                    else if (userRoles.Contains("Editor"))
                    {
                        users = context.SystemUser.Where(p => p.ID == BackendUserHelper.CurrentUserLiteData.ID).ToList();
                    }
                    else
                    {
                        users = null;
                    }
                }
            }
            return users;
        }

        public List<ColumnType> GetColTypeList()
        {
            return QueryFor<ColumnType>().Where(p => p.ID > 0).ToList();
        }

        public List<RegViewModel> GetRegList(Event ev)
        {
            List < RegViewModel > regViewList=new List<RegViewModel>();
            foreach (EventRegist eg in ev.EventRegist)
            {
                RegViewModel regView = new RegViewModel();
                regView.ID = eg.ID;
                regView.EventId = eg.EventId;
                regView.OrderNum = eg.OrderNum == null ? 0 : eg.OrderNum;
                regView.IsAttend = eg.IsAttend;
                regView.IsContact = eg.IsContact;
                regView.CreateDate = eg.CreateDate;
                regView.ContactDate = eg.ContactDate;

                regView.ColValues=new Dictionary<int, string>();
                if (string.IsNullOrEmpty(eg.Text01))
                {
                    continue;
                }
                string[] egStrings = eg.Text01.Split(new string[] {"^}"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var egstr in egStrings)
                {
                    if (string.IsNullOrEmpty(egstr))
                    {
                        continue;                        
                    }
                    string optVal = "";
                    string[] esstrs = egstr.Split(new string[] { "&=" }, StringSplitOptions.RemoveEmptyEntries);
                    
                    int esid = -1;
                    Int32.TryParse(esstrs[0], out esid);
                    string optstr = esstrs[1];
                    
                    if (esid == -1 && string.IsNullOrEmpty(optstr))
                    {
                        continue;
                    }
                    EventSign es = this.QueryFor<EventSign>().FirstOrDefault(p => p.ID == esid);
                    if (es==null )
                    {
                        continue;
                    }
                    if (es.TypeId==4)
                    {
                        int i = -1;
                        Int32.TryParse(optstr, out i);
                        if (i==-1)
                        {
                            continue;
                        }
                        optVal =
                            this.QueryFor<EventDropList>()
                                .Where(p => p.EventSignId == es.ID)
                                .FirstOrDefault(p => p.ID == i)
                                .RowValue;
                    }
                    else
                    {
                        optVal = optstr;
                    }
                    regView.RegValues += ","+ esid + ":\'" + optVal + "\'";
                    regView.ColValues.Add(esid, optVal);
                }
                regView.RegValues += "}";
                regViewList.Add(regView);
            }
            return regViewList;
        }

        public EventRegist UpdateRegContact(int id, string isContacted, string MailList)
        {
            EventRegist er = null;
            bool isNeedMail = false;
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    er = context.EventRegist.FirstOrDefault(p => p.ID == id);
                    if (er!=null)
                    {
                        if (isContacted=="checked")
                        {
                            isNeedMail = true;
                        }
                        er.IsContact = isContacted == "checked" ? true : false;
                        er.ContactDate = DateTime.Now;
                        if (isContacted!="checked")
                        {
                            er.OrderNum = 0;
                        }
                        else
                        {
                            int oNum = context.EventRegist.Max(p =>(int) p.OrderNum);
                            if (oNum<=0)
                            {
                                oNum = context.EventRegist.Count(p => p.EventId == er.EventId && p.OrderNum > 0 && p.ID != id);
                            }
                            er.OrderNum = oNum + 1;
                        }
                        if (er.OrderNum>0 && isNeedMail)
                        {
                            string scanUrl= ConfigurationManager.AppSettings["PublicSiteUrl"]+ "Announce/ScanConfirm?EventId=" + er.EventId + "&EvRegId=" + er.ID;
                            er.Text02=ImageHelper.GenerateQRcodeImg(scanUrl);
                        }
                        
                        context.SaveChanges();

                        var validEmails = NotifyHelper.IsValidEmailList(MailList);

                        if (isNeedMail && validEmails.Count>0)
                        {
                            Event ev = context.Event.FirstOrDefault(p => p.ID == er.EventId);
                            if (ev != null)
                            {
                                if (!string.IsNullOrEmpty(ev.CfmEmailContent))
                                {
                                    string mailMsg = ev.CfmEmailContent;
                                    mailMsg =mailMsg.Replace(@"{thisname}", ev.Name).Replace(@"{address}", ev.Address).Replace(@"{checkin}", er.OrderNum.ToString());
                                    SystemSetting setting = NotifyHelper.GetSystemSetting();
                                    string qrCodePath = "";
                                    if (er.Event.QRcodeMode == 2 && !string.IsNullOrWhiteSpace(er.Text02))
                                    {
                                        qrCodePath = er.Text02;
                                    }
                                    Task.Factory.StartNew(() =>
                                    {
                                        NotifyHelper.SendMail(setting, validEmails, "Your Activity Application have been Confirmed", mailMsg, true, qrCodePath);
                                    });
                                }
                            }
                        }
                    }                    
                }
                scope.Complete();
            }
            return er;
        }
        public EventRegist UpdateRegAttend(int id, string isAttend)
        {
            EventRegist er = null;
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    er = context.EventRegist.FirstOrDefault(p => p.ID == id);
                    if (er != null)
                    {
                        er.IsAttend = isAttend == "checked" ? true : false;                       
                        context.SaveChanges();
                    }
                }
                scope.Complete();
            }
            return er;
        }
        public byte[] ExportRegList(List<RegViewModel> regList, Dictionary<int, string> colTitles)
        {
            IWorkbook workbook = new HSSFWorkbook();
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
            cellStyle.FillForegroundColor = HSSFColor.Grey40Percent.Index;
            List<string>columns=new List<string>() { "報名確認", "參加確認", "報名序號" };

            colTitles.OrderBy(p => p.Key);
            foreach (var col in colTitles)
            {
                columns.Add(col.Value);
            }

            //在Excel檔上通過對HSSFSheet創建一個工作表
            ISheet sheet = workbook.CreateSheet("Sheet1");
            //給工作表上添加一行
            IRow row0 = sheet.CreateRow(0);

            int index = 0;
            foreach (var column in columns)
            {
                row0.CreateCell(index).SetCellValue(column);
                row0.GetCell(index).CellStyle = cellStyle;
                if (index < columns.Count)
                {
                    index++;
                }
            }
            row0.HeightInPoints = (float)1.5 * sheet.DefaultRowHeight / 20;

            for (int j = 0; j < regList.Count; j++)
            {
                var item = regList[j];
                var row = sheet.CreateRow(j + 1);

                row.CreateCell(0).SetCellValue(item.IsContact==true?"YES":"NO");
                row.CreateCell(1).SetCellValue(item.IsAttend == true ? "YES" : "NO");
                row.CreateCell(2).SetCellValue((double)item.OrderNum);
                int k = 3;
                foreach (KeyValuePair<int, string> colt in colTitles)
                {
                    string rowValue = "";
                    item.ColValues.TryGetValue(colt.Key, out rowValue);
                    if (!String.IsNullOrEmpty(rowValue))
                    {
                        row.CreateCell(k).SetCellValue(rowValue);
                    }
                    k++;
                }


                row.HeightInPoints = (float)1.5 * sheet.DefaultRowHeight / 20;
            }

            //設置單元的寬度
            for (int i = 0; i <= colTitles.Count+3; i++)
            {
                sheet.SetColumnWidth(i, 20 * 256);
            }

            var stream = new MemoryStream();
            workbook.Write(stream);
            stream.Flush();
            stream.Close();

            return stream.ToArray();
        }        

    }
}
