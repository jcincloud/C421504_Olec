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
    public class NewsController : ajaxApi<News, q_News>
    {
        public async Task<IHttpActionResult> Get(int id)
        {
            using (db0 = getDB0())
            {
                item = await db0.News.FindAsync(id);
                r = new ResultInfo<News>() { data = item };
            }

            return Ok(r);
        }
        public async Task<IHttpActionResult> Get([FromUri]q_News q)
        {
            #region 連接BusinessLogicLibary資料庫並取得資料

            using (db0 = getDB0())
            {
                var qr = db0.News
                    .OrderByDescending(x => x.news_date).AsQueryable();


                if (q.title != null)
                {
                    qr = qr.Where(x => x.news_title.Contains(q.title));
                }

                if (q.i_Lang != null)
                {
                    qr = qr.Where(x => x.i_Lang == q.i_Lang);
                }

                var result = qr.Select(x => new m_News()
                {
                    news_id = x.news_id,
                    news_title = x.news_title,
                    news_date = x.news_date,
                    i_Hide = x.i_Hide,
                    i_Lang = x.i_Lang
                });


                int page = (q.page == null ? 1 : (int)q.page);
                int position = PageCount.PageInfo(page, this.defPageSize, qr.Count());
                var segment = await result.Skip(position).Take(this.defPageSize).ToListAsync();

                return Ok<GridInfo<m_News>>(new GridInfo<m_News>()
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
        public async Task<IHttpActionResult> Put([FromBody]News md)
        {
            ResultInfo r = new ResultInfo();
            try
            {
                db0 = getDB0();

                item = await db0.News.FindAsync(md.news_id);
                item.news_title = md.news_title;
                item.news_intro = md.news_intro;
                item.news_date = md.news_date;
                item.news_content = md.news_content;
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
        public async Task<IHttpActionResult> Post([FromBody]News md)
        {
            md.news_id = GetNewId(ProcCore.Business.CodeTable.News);
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

                db0.News.Add(md);
                await db0.SaveChangesAsync();

                r.result = true;
                r.id = md.news_id;
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
                    item = new News() { news_id = id };
                    db0.News.Attach(item);
                    db0.News.Remove(item);
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
