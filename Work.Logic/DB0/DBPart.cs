﻿using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
namespace ProcCore.Business.DB0
{
    public enum EditState
    {
        Insert = 0,
        Update = 1
    }
    public enum VisitDetailState
    {
        none,
        wait,
        progress,
        finish,
        pause
    }
    #region set CodeSheet

    public static class CodeSheet
    {
        public static List<i_Code> visitdetail_state = new List<i_Code>()
        {
            new i_Code{ Code = 0, Value = "無狀態", LangCode = "none" },
            new i_Code{ Code = 1, Value = "未拜訪", LangCode = "wait" },
            new i_Code{ Code = 2, Value = "進行中", LangCode = "progress" },
            new i_Code{ Code = 3, Value = "完成", LangCode = "finish" },
            new i_Code{ Code = 4, Value = "暫停", LangCode = "pause" }
        };
        public static List<i_Code> customer_type = new List<i_Code>()
        {
            new i_Code{ Code = 0, Value = "無", LangCode = "none" },
            new i_Code{ Code = 1, Value = "店家", LangCode = "store" },
            new i_Code{ Code = 2, Value = "直客", LangCode = "straght" }
        };
        public static List<i_Code> channel_type = new List<i_Code>()
        {
            new i_Code{ Code = 0, Value = "無", LangCode = "none" },
            new i_Code{ Code = 1, Value = "即飲", LangCode = "" },
            new i_Code{ Code = 2, Value = "外帶", LangCode = "" }
        };
        public static List<i_Code> store_type = new List<i_Code>()
        {
            new i_Code{ Code = 0, Value = "無", LangCode = "none" },
            new i_Code{ Code = 1, Value = "LS", LangCode = "" },
            new i_Code{ Code = 2, Value = "Beer Store", LangCode = "" },
            new i_Code{ Code = 3, Value = "Dancing", LangCode = "" },
            new i_Code{ Code = 4, Value = "Bar", LangCode = "" },
            new i_Code{ Code = 5, Value = "Cafe", LangCode = "" },
            new i_Code{ Code = 6, Value = "Bistro", LangCode = "" },
            new i_Code{ Code = 7, Value = "Restaurant", LangCode = "" }
        };
        public static List<i_Code> store_level = new List<i_Code>()
        {
            new i_Code{ Code = 0, Value = "無", LangCode = "none" },
            new i_Code{ Code = 1, Value = "G", LangCode = "" },
            new i_Code{ Code = 2, Value = "S", LangCode = "" },
            new i_Code{ Code = 3, Value = "B", LangCode = "" }
        };
        public static List<i_Code> evaluate = new List<i_Code>()
        {
            new i_Code{ Code = null, Value = "無", LangCode = "none" },
            new i_Code{ Code = 0, Value = "無", LangCode = "none" },
            new i_Code{ Code = 1, Value = "A", LangCode = "" },
            new i_Code{ Code = 2, Value = "B", LangCode = "" },
            new i_Code{ Code = 3, Value = "C", LangCode = "" }
        };
        public static string GetStateVal(int code)
        {
            string Val = string.Empty;
            foreach (var item in visitdetail_state)
            {
                if (item.Code == code)
                    Val = item.Value;
            }
            return Val;
        }
        public static string GetCustomerTypeVal(int code)
        {
            string Val = string.Empty;
            foreach (var item in customer_type)
            {
                if (item.Code == code)
                    Val = item.Value;
            }
            return Val;
        }

        public static string GetChannelTypeVal(int code)
        {
            string Val = string.Empty;
            foreach (var item in channel_type)
            {
                if (item.Code == code)
                    Val = item.Value;
            }
            return Val;
        }
        public static string GetStoreTypeVal(int code)
        {
            string Val = string.Empty;
            foreach (var item in store_type)
            {
                if (item.Code == code)
                    Val = item.Value;
            }
            return Val;
        }
        public static string GetStoreLevelVal(int? code)
        {
            string Val = string.Empty;
            foreach (var item in store_level)
            {
                if (item.Code == code)
                    Val = item.Value;
            }
            return Val;
        }
        public static string GetEvaluateVal(int? code)
        {
            string Val = string.Empty;
            foreach (var item in evaluate)
            {
                if (item.Code == code)
                    Val = item.Value;
            }
            return Val;
        }
    }
    public class i_Code
    {
        public int? Code { get; set; }
        public string LangCode { get; set; }
        public string Value { get; set; }
    }
    #endregion

    public partial class C42A0_OlecEntities : DbContext
    {
        public C42A0_OlecEntities(string connectionstring)
            : base(connectionstring)
        {
        }

        public override Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                Log.Write(ex.Message, ex.StackTrace);
                foreach (var err_Items in ex.EntityValidationErrors)
                {
                    foreach (var err_Item in err_Items.ValidationErrors)
                    {
                        Log.Write("欄位驗證錯誤", err_Item.PropertyName, err_Item.ErrorMessage);
                    }
                }

                throw ex;
            }
            catch (DbUpdateException ex)
            {
                Log.Write("DbUpdateException", ex.InnerException.Message);
                throw ex;
            }
            catch (EntityException ex)
            {
                Log.Write("EntityException", ex.Message);
                throw ex;
            }
            catch (UpdateException ex)
            {
                Log.Write("UpdateException", ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Write("Exception", ex.Message);
                throw ex;
            }
        }

    }

    #region Model Expand
    public class DocInfo
    {
        public String Name { get; set; }
        public int Sort { get; set; }
        public String Momo { get; set; }
        public String Link { get; set; }
    }
    public partial class News : BaseEntityTable
    {
        public string imgsrc { get; set; }
    }
    public class NewsInfo
    {
        public IList<News> News { get; set; }
        public IList<string> title { get; set; }
    }
    public class NewsContent : NewsInfo
    {
        public News item { get; set; }
    }
    public class ProductInfo
    {
        public Product Product { get; set; }
        public IList<ProductStandard> ProductStandard { get; set; }
        public IList<Plist> list { get; set; }
    }
    public class Plist
    {
        public string p_name { get; set; }
        public int p_id { get; set; }
    }
    #endregion

    #region q_Model_Define
    public class q_AspNetRoles : QueryBase
    {
        public string Name { set; get; }

    }
    public class q_AspNetUsers : QueryBase
    {
        public string UserName { set; get; }

    }
    public class q_OtherUnit : QueryBase
    {
        public string name { get; set; }
        public int? category { get; set; }
        public bool? state { get; set; }
        public int? hot { get; set; }

    }
    public class q_Product : QueryBase
    {
        public string product_name { get; set; }
        public int? product_category_id { get; set; }
        public string i_Lang { get; set; }
    }
    public class q_ProductStandard : QueryBase
    {
        public int? main_id { get; set; }
    }
    public class q_Customer : QueryBase
    {
        public string i_InsertUserID { get; set; }
        public DateTime? i_InsertDateTime { get; set; }
        public string customer_name { get; set; }
        public bool? mark_err { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public int? area { get; set; }
        public string address { get; set; }
        public string tel { get; set; }
    }
    public class q_News : QueryBase
    {
        public string title { get; set; }
        public DateTime date { get; set; }
        public string i_Lang { get; set; }
    }
    #endregion

    #region c_Model_Define
    public class c_AspNetRoles
    {
        public q_AspNetRoles q { get; set; }
        public AspNetRoles m { get; set; }
    }
    public partial class c_AspNetUsers
    {
        public q_AspNetUsers q { get; set; }
        public AspNetUsers m { get; set; }
    }
    #endregion
}
