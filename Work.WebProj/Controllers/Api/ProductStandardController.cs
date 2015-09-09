using DotWeb.Helpers;
using ProcCore.Business.DB0;
using ProcCore.HandleResult;
using ProcCore.WebCore;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotWeb.Api
{
    public class ProductStandardController : ajaxApi<ProductStandard, q_ProductStandard>
    {
        public async Task<IHttpActionResult> Get(int id)
        {
            using (db0 = getDB0())
            {
                item = await db0.ProductStandard.FindAsync(id);
                r = new ResultInfo<ProductStandard>() { data = item };
            }

            return Ok(r);
        }
        public async Task<IHttpActionResult> Get([FromUri]q_ProductStandard q)
        {
            #region 連接BusinessLogicLibary資料庫並取得資料

            using (db0 = getDB0())
            {
                var items = (from x in db0.ProductStandard
                             orderby x.sort descending
                             where x.product_id == q.main_id
                             select new m_ProductStandard()
                             {
                                 product_id = x.product_id,
                                 standard_id = x.standard_id,
                                 appearance = x.appearance,
                                 viscosity = x.viscosity,
                                 item_no=x.item_no,
                                 soften_point=x.soften_point,
                                 remark=x.remark,
                                 sort = x.sort,
                                 i_Hide = x.i_Hide,
                                 i_Lang = x.i_Lang,
                                 edit_state = EditState.Update
                             });

                return Ok(items.ToList());
            }
            #endregion
        }
        public async Task<IHttpActionResult> Put([FromBody]ProductStandard md)
        {
            ResultInfo r = new ResultInfo();
            try
            {
                db0 = getDB0();

                item = await db0.ProductStandard.FindAsync(md.standard_id);
                item.item_no = md.item_no;
                item.appearance = md.appearance;
                item.viscosity = md.viscosity;
                item.soften_point = md.soften_point;
                item.remark = md.remark;
                item.sort = md.sort;
                item.i_Lang = md.i_Lang;
                item.i_Hide = md.i_Hide;

                item.i_UpdateUserID = this.UserId;
                item.i_UpdateDateTime = DateTime.Now;
                item.i_UpdateDeptID = this.departmentId;

                await db0.SaveChangesAsync();
                r.result = true;
            }
            catch (Exception ex)
            {
                r.result = false;
                r.message = ex.ToString();
            }
            finally
            {
                db0.Dispose();
            }
            return Ok(r);
        }
        public async Task<IHttpActionResult> Post([FromBody]ProductStandard md)
        {
            md.standard_id = GetNewId(ProcCore.Business.CodeTable.Product);
            ResultInfo r = new ResultInfo();
            if (!ModelState.IsValid)
            {
                r.message = ModelStateErrorPack();
                r.result = false;
                return Ok(r);
            }

            try
            {
                #region working a
                db0 = getDB0();

                md.i_InsertUserID = this.UserId;
                md.i_InsertDateTime = DateTime.Now;
                md.i_InsertDeptID = this.departmentId;
                //md.i_Lang = "zh-TW";

                db0.ProductStandard.Add(md);
                await db0.SaveChangesAsync();

                r.result = true;
                r.id = md.product_id;
                return Ok(r);
                #endregion
            }
            catch (Exception ex)
            {
                r.result = false;
                r.message = ex.Message;
                return Ok(r);
            }
            finally
            {
                db0.Dispose();
            }
        }
        public async Task<IHttpActionResult> Delete([FromUri]int[] ids)
        {
            ResultInfo r = new ResultInfo();
            try
            {
                db0 = getDB0();

                foreach (var id in ids)
                {
                    item = new ProductStandard() { standard_id = id };
                    db0.ProductStandard.Attach(item);
                    db0.ProductStandard.Remove(item);
                }

                await db0.SaveChangesAsync();

                r.result = true;
                return Ok(r);
            }
            catch (DbUpdateException ex)
            {
                r.result = false;
                if (ex.InnerException != null)
                {
                    r.message = Resources.Res.Log_Err_Delete_DetailExist
                        + "\r\n" + getErrorMessage(ex);
                }
                else
                {
                    r.message = ex.Message;
                }
                return Ok(r);
            }
            catch (Exception ex)
            {
                r.result = false;
                r.message = ex.Message;
                return Ok(r);
            }
            finally
            {
                db0.Dispose();
            }
        }
    }
}
