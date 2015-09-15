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
    public class ProductController : ajaxApi<Product, q_Product>
    {
        public async Task<IHttpActionResult> Get(int id)
        {
            using (db0 = getDB0())
            {
                item = await db0.Product.FindAsync(id);
                r = new ResultInfo<Product>() { data = item };
            }

            return Ok(r);
        }
        public async Task<IHttpActionResult> Get([FromUri]q_Product q)
        {
            #region 連接BusinessLogicLibary資料庫並取得資料

            using (db0 = getDB0())
            {
                var qr = db0.Product
                    .OrderByDescending(x => x.product_id).AsQueryable();


                if (q.product_name != null)
                {
                    qr = qr.Where(x => x.product_name.Contains(q.product_name));
                }

                if (q.i_Lang != null)
                {
                    qr = qr.Where(x => x.i_Lang == q.i_Lang);
                }

                var result = qr.Select(x => new m_Product()
                {
                    product_id = x.product_id,
                    product_name = x.product_name,
                    i_Hide = x.i_Hide,
                    i_Lang = x.i_Lang
                });


                int page = (q.page == null ? 1 : (int)q.page);
                int position = PageCount.PageInfo(page, this.defPageSize, qr.Count());
                var segment = await result.Skip(position).Take(this.defPageSize).ToListAsync();

                return Ok<GridInfo<m_Product>>(new GridInfo<m_Product>()
                {
                    rows = segment,
                    total = PageCount.TotalPage,
                    page = PageCount.Page,
                    records = PageCount.RecordCount,
                    startcount = PageCount.StartCount,
                    endcount = PageCount.EndCount
                });
            }
            #endregion
        }
        public async Task<IHttpActionResult> Put([FromBody]Product md)
        {
            ResultInfo r = new ResultInfo();
            try
            {
                db0 = getDB0();


                item = await db0.Product.FindAsync(md.product_id);

                item.product_name = md.product_name;
                item.product_info = md.product_info;
                item.product_note = md.product_note;
                item.sort = md.sort;
                item.i_Lang = md.i_Lang;
                item.i_Hide = md.i_Hide;

                item.i_UpdateUserID = this.UserId;
                item.i_UpdateDateTime = DateTime.Now;
                item.i_UpdateDeptID = this.departmentId;


                var details = item.ProductStandard;

                foreach (var detail in details)
                {
                    var md_detail = md.ProductStandard.First(x => x.standard_id == detail.standard_id);
                    detail.item_no = md_detail.item_no;
                    detail.appearance = md_detail.appearance;
                    detail.viscosity = md_detail.viscosity;
                    detail.soften_point = md_detail.soften_point;
                    detail.remark = md_detail.remark;
                    detail.sort = md_detail.sort;
                    detail.i_Lang = md.i_Lang;//主檔語系變就跟著變
                }

                var add_detail = md.ProductStandard.Where(x => x.edit_state == EditState.Insert);
                foreach (var detail in add_detail)
                {
                    detail.standard_id = GetNewId(ProcCore.Business.CodeTable.ProductStandard);
                    detail.product_id = md.product_id;
                    detail.i_InsertUserID = this.UserId;
                    detail.i_InsertDateTime = DateTime.Now;
                    detail.i_InsertDeptID = this.departmentId;
                    detail.i_Lang = md.i_Lang;//主檔語系變就跟著變
                    details.Add(detail);
                }

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
        public async Task<IHttpActionResult> Post([FromBody]Product md)
        {
            md.product_id = GetNewId(ProcCore.Business.CodeTable.Product);
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


                foreach (var detail in md.ProductStandard)
                {
                    detail.standard_id = GetNewId(ProcCore.Business.CodeTable.ProductStandard);
                    detail.product_id = md.product_id;
                    detail.i_InsertUserID = this.UserId;
                    detail.i_InsertDateTime = DateTime.Now;
                    detail.i_InsertDeptID = this.departmentId;
                    detail.i_Lang = md.i_Lang;//主檔語系變就跟著變
                    db0.ProductStandard.Add(detail);
                }


                db0.Product.Add(md);
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
                    var getDetails = db0.ProductStandard.Where(x => x.product_id == id);
                    foreach (var details in getDetails)
                    {//先刪除底層產品規格
                        db0.ProductStandard.Attach(details);
                        db0.ProductStandard.Remove(details);
                    }
                    item = new Product() { product_id = id };
                    db0.Product.Attach(item);
                    db0.Product.Remove(item);
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
