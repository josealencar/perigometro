using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using Perigometro.Dominio;
using WebApplication1.Models;

namespace WebApplication1
{   
    [Authorize]
    public class AcidentesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Acidentes
        public ActionResult Index()
        {
            return View(db.Acidentes.ToList());
        }

        // GET: Acidentes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acidente acidente = db.Acidentes.Find(id);
            if (acidente == null)
            {
                return HttpNotFound();
            }
            return View(acidente);
        }

        // GET: Acidentes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Acidentes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Acidente acidente)
        {
            if (ModelState.IsValid)
            {
                db.Acidentes.Add(acidente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(acidente);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult CreateLista(List<Acidente> acidentes)
        {
            if (acidentes == null) return Json( "Falhou" ,JsonRequestBehavior.AllowGet);
            foreach (var acidente in acidentes)
            {
                db.Acidentes.Add(acidente);      
            }
            db.SaveChanges();
            return Json("Salvou no BD", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult BuscarInicial()
        {
            var dados = db.Acidentes.Where(_ => _.Ano == 2013).Select(_ => new { _.Latitude, _.Longitude }).ToList();
            var jsonResult = Json(new { Dados = dados}, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        // GET: Acidentes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acidente acidente = db.Acidentes.Find(id);
            if (acidente == null)
            {
                return HttpNotFound();
            }
            return View(acidente);
        }

        // POST: Acidentes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Latitude,Longitude,Tipo_Acid,Fatais,Auto,Taxi,Lotacao,Onibus_Urb,Onibus_Int,Caminhao,Moto,Carroca,Bicicleta,Outro,Tempo,Noite_dia,Regiao,Dia_Sem,Ano")] Acidente acidente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(acidente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(acidente);
        }

        // GET: Acidentes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acidente acidente = db.Acidentes.Find(id);
            if (acidente == null)
            {
                return HttpNotFound();
            }
            return View(acidente);
        }

        // POST: Acidentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Acidente acidente = db.Acidentes.Find(id);
            db.Acidentes.Remove(acidente);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
