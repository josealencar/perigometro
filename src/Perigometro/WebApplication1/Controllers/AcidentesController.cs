using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using Perigometro.Dominio1;
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
            var dados = db.Acidentes.OrderByDescending(x => x.Id).Take(100).ToList();
            return View(dados);
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
        
        [AllowAnonymous]
        public JsonResult BuscarDadosFiltrados()
        {
            #region RequestDadosForm
            var anoInicial = Request.Form.Get("AnoInicial");
            var anoFinal = Request.Form.Get("AnoFinal");
            var clima = Request.Form.Get("Clima");
            var regiao = Request.Form.Get("Regiao");
            var semana = Request.Form.Get("Semana");
            var turno = Request.Form.Get("Turno");
            var tipo = Request.Form.Get("Tipo");
            var veiculo = Request.Form.Get("Veiculo");
            var fatal = Request.Form.Get("Fatal");
            #endregion

            #region booleanos
            bool anoInicialPreenchido = anoInicial != null && anoInicial != string.Empty;
            bool anoFinalPreenchido = anoFinal != null && anoFinal != string.Empty;
            bool climaPreenchido = clima != null && clima != string.Empty;
            bool regiaoPreenchido = regiao != null && regiao != string.Empty;
            bool semanaPreenchido = semana != null && semana != string.Empty;
            bool turnoPreenchido = turno != null && turno != string.Empty;
            bool tipoPreenchido = tipo != null && tipo != string.Empty;
            bool veiculoPreenchido = veiculo != null && veiculo != string.Empty;
            bool fatalPreenchido = fatal != null && fatal != string.Empty;
            #endregion

            #region funcoesWhereEncadeadas
            #region AmbosAnosPreenchidos
            if (anoInicialPreenchido && anoFinalPreenchido)
            {
                var anoInicialSelecionado = Convert.ToInt32(anoInicial);
                var anoFinalSelecionado = Convert.ToInt32(anoFinal);
                #region AnoInicialMenorQueFinal
                if (anoInicialSelecionado < anoFinalSelecionado)
                {
                    var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado && _.Ano <= anoFinalSelecionado);
                    if (climaPreenchido)
                    {
                        var climaSelecionado = Enum.GetName(typeof(Clima), Convert.ToInt32(clima)).ToUpper();
                        dados = dados.Where(_ => _.Tempo == climaSelecionado);
                    }
                    if (regiaoPreenchido)
                    {
                        var regiaoSelecionado = Enum.GetName(typeof(Regiao), Convert.ToInt32(regiao)).ToUpper();
                        dados = dados.Where(_ => _.Regiao == regiaoSelecionado);
                    }
                    if (semanaPreenchido)
                    {
                        var semanaSelecionado = Enum.GetName(typeof(Semana), Convert.ToInt32(semana)).ToUpper() + "-FEIRA";
                        dados = dados.Where(_ => _.Dia_Sem == semanaSelecionado);
                    }
                    if (turnoPreenchido)
                    {
                        var turnoSelecionado = Enum.GetName(typeof(Turno), Convert.ToInt32(turno)).ToUpper();
                        dados = dados.Where(_ => _.Noite_dia == turnoSelecionado);
                    }
                    if (tipoPreenchido)
                    {
                        var tipoSelecionado = Enum.GetName(typeof(Tipo), Convert.ToInt32(tipo)).ToUpper();
                        dados = dados.Where(_ => _.Tipo_Acid == tipoSelecionado);
                    }
                    if (veiculoPreenchido)
                    {
                        var veiculoSelecionado = Enum.GetName(typeof(Veiculo), Convert.ToInt32(veiculo));
                        if (veiculoSelecionado.Equals("Automovel")) dados = dados.Where(_ => _.Auto > 0);
                        else if (veiculoSelecionado.Equals("Taxi")) dados = dados.Where(_ => _.Taxi > 0);
                        else if (veiculoSelecionado.Equals("Lotacao")) dados = dados.Where(_ => _.Lotacao > 0);
                        else if (veiculoSelecionado.Equals("OnibusUrb")) dados = dados.Where(_ => _.Onibus_Urb > 0);
                        else if (veiculoSelecionado.Equals("OnibusMet")) dados = dados.Where(_ => _.Onibus_Met > 0);
                        else if (veiculoSelecionado.Equals("OnibusInt")) dados = dados.Where(_ => _.Onibus_Int > 0);
                        else if (veiculoSelecionado.Equals("Caminhao")) dados = dados.Where(_ => _.Caminhao > 0);
                        else if (veiculoSelecionado.Equals("Moto")) dados = dados.Where(_ => _.Moto > 0);
                        else if (veiculoSelecionado.Equals("Carroca")) dados = dados.Where(_ => _.Carroca > 0);
                        else if (veiculoSelecionado.Equals("Bicicleta")) dados = dados.Where(_ => _.Bicicleta > 0);
                        else if (veiculoSelecionado.Equals("Outro")) dados = dados.Where(_ => _.Outro > 0);
                    }
                    if (fatalPreenchido)
                    {
                        dados = dados.Where(_ => _.Fatais > 0);
                    }
                    var dadosJson = dados.Select(_ => new { _.Latitude, _.Longitude }).ToList();
                    var jsonResult = Json(new { Dados = dadosJson }, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                #endregion
                #region AnoInicialMaiorQueFinal
                if (anoInicialSelecionado > anoFinalSelecionado)
                {
                    var dados = db.Acidentes.Where(_ => _.Ano <= anoInicialSelecionado && _.Ano >= anoFinalSelecionado);
                    if (climaPreenchido)
                    {
                        var climaSelecionado = Enum.GetName(typeof(Clima), Convert.ToInt32(clima)).ToUpper();
                        dados = dados.Where(_ => _.Tempo == climaSelecionado);
                    }
                    if (regiaoPreenchido)
                    {
                        var regiaoSelecionado = Enum.GetName(typeof(Regiao), Convert.ToInt32(regiao)).ToUpper();
                        dados = dados.Where(_ => _.Regiao == regiaoSelecionado);
                    }
                    if (semanaPreenchido)
                    {
                        var semanaSelecionado = Enum.GetName(typeof(Semana), Convert.ToInt32(semana)).ToUpper() + "-FEIRA";
                        dados = dados.Where(_ => _.Dia_Sem == semanaSelecionado);
                    }
                    if (turnoPreenchido)
                    {
                        var turnoSelecionado = Enum.GetName(typeof(Turno), Convert.ToInt32(turno)).ToUpper();
                        dados = dados.Where(_ => _.Noite_dia == turnoSelecionado);
                    }
                    if (tipoPreenchido)
                    {
                        var tipoSelecionado = Enum.GetName(typeof(Tipo), Convert.ToInt32(tipo)).ToUpper();
                        dados = dados.Where(_ => _.Tipo_Acid == tipoSelecionado);
                    }
                    if (veiculoPreenchido)
                    {
                        var veiculoSelecionado = Enum.GetName(typeof(Veiculo), Convert.ToInt32(veiculo));
                        if (veiculoSelecionado.Equals("Automovel")) dados = dados.Where(_ => _.Auto > 0);
                        else if (veiculoSelecionado.Equals("Taxi")) dados = dados.Where(_ => _.Taxi > 0);
                        else if (veiculoSelecionado.Equals("Lotacao")) dados = dados.Where(_ => _.Lotacao > 0);
                        else if (veiculoSelecionado.Equals("OnibusUrb")) dados = dados.Where(_ => _.Onibus_Urb > 0);
                        else if (veiculoSelecionado.Equals("OnibusMet")) dados = dados.Where(_ => _.Onibus_Met > 0);
                        else if (veiculoSelecionado.Equals("OnibusInt")) dados = dados.Where(_ => _.Onibus_Int > 0);
                        else if (veiculoSelecionado.Equals("Caminhao")) dados = dados.Where(_ => _.Caminhao > 0);
                        else if (veiculoSelecionado.Equals("Moto")) dados = dados.Where(_ => _.Moto > 0);
                        else if (veiculoSelecionado.Equals("Carroca")) dados = dados.Where(_ => _.Carroca > 0);
                        else if (veiculoSelecionado.Equals("Bicicleta")) dados = dados.Where(_ => _.Bicicleta > 0);
                        else if (veiculoSelecionado.Equals("Outro")) dados = dados.Where(_ => _.Outro > 0);
                    }
                    if (fatalPreenchido)
                    {
                        dados = dados.Where(_ => _.Fatais > 0);
                    }
                    var dadosJson = dados.Select(_ => new { _.Latitude, _.Longitude }).ToList();
                    var jsonResult = Json(new { Dados = dadosJson }, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                #endregion
                #region AnoInicialIgualAoFinal
                if (anoInicialSelecionado == anoFinalSelecionado)
                {
                    var dados = db.Acidentes.Where(_ => _.Ano == anoInicialSelecionado);
                    if (climaPreenchido)
                    {
                        var climaSelecionado = Enum.GetName(typeof(Clima), Convert.ToInt32(clima)).ToUpper();
                        dados = dados.Where(_ => _.Tempo == climaSelecionado);
                    }
                    if (regiaoPreenchido)
                    {
                        var regiaoSelecionado = Enum.GetName(typeof(Regiao), Convert.ToInt32(regiao)).ToUpper();
                        dados = dados.Where(_ => _.Regiao == regiaoSelecionado);
                    }
                    if (semanaPreenchido)
                    {
                        var semanaSelecionado = Enum.GetName(typeof(Semana), Convert.ToInt32(semana)).ToUpper() + "-FEIRA";
                        dados = dados.Where(_ => _.Dia_Sem == semanaSelecionado);
                    }
                    if (turnoPreenchido)
                    {
                        var turnoSelecionado = Enum.GetName(typeof(Turno), Convert.ToInt32(turno)).ToUpper();
                        dados = dados.Where(_ => _.Noite_dia == turnoSelecionado);
                    }
                    if (tipoPreenchido)
                    {
                        var tipoSelecionado = Enum.GetName(typeof(Tipo), Convert.ToInt32(tipo)).ToUpper();
                        dados = dados.Where(_ => _.Tipo_Acid == tipoSelecionado);
                    }
                    if (veiculoPreenchido)
                    {
                        var veiculoSelecionado = Enum.GetName(typeof(Veiculo), Convert.ToInt32(veiculo));
                        if (veiculoSelecionado.Equals("Automovel")) dados = dados.Where(_ => _.Auto > 0);
                        else if (veiculoSelecionado.Equals("Taxi")) dados = dados.Where(_ => _.Taxi > 0);
                        else if (veiculoSelecionado.Equals("Lotacao")) dados = dados.Where(_ => _.Lotacao > 0);
                        else if (veiculoSelecionado.Equals("OnibusUrb")) dados = dados.Where(_ => _.Onibus_Urb > 0);
                        else if (veiculoSelecionado.Equals("OnibusMet")) dados = dados.Where(_ => _.Onibus_Met > 0);
                        else if (veiculoSelecionado.Equals("OnibusInt")) dados = dados.Where(_ => _.Onibus_Int > 0);
                        else if (veiculoSelecionado.Equals("Caminhao")) dados = dados.Where(_ => _.Caminhao > 0);
                        else if (veiculoSelecionado.Equals("Moto")) dados = dados.Where(_ => _.Moto > 0);
                        else if (veiculoSelecionado.Equals("Carroca")) dados = dados.Where(_ => _.Carroca > 0);
                        else if (veiculoSelecionado.Equals("Bicicleta")) dados = dados.Where(_ => _.Bicicleta > 0);
                        else if (veiculoSelecionado.Equals("Outro")) dados = dados.Where(_ => _.Outro > 0);
                    }
                    if (fatalPreenchido)
                    {
                        dados = dados.Where(_ => _.Fatais > 0);
                    }
                    var dadosJson = dados.Select(_ => new { _.Latitude, _.Longitude }).ToList();
                    var jsonResult = Json(new { Dados = dadosJson }, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                #endregion
            }
            #endregion
            #region ApenasUmAnoPreenchido
            if (anoInicialPreenchido || anoFinalPreenchido)
            {
                var anoSelecionado = anoInicialPreenchido ?
                    Convert.ToInt32(anoInicial) : Convert.ToInt32(anoFinal);
                var dados = db.Acidentes.Where(_ => _.Ano == anoSelecionado);
                if (climaPreenchido)
                {
                    var climaSelecionado = Enum.GetName(typeof(Clima), Convert.ToInt32(clima)).ToUpper();
                    dados = dados.Where(_ => _.Tempo == climaSelecionado);
                }
                if (regiaoPreenchido)
                {
                    var regiaoSelecionado = Enum.GetName(typeof(Regiao), Convert.ToInt32(regiao)).ToUpper();
                    dados = dados.Where(_ => _.Regiao == regiaoSelecionado);
                }
                if (semanaPreenchido)
                {
                    var semanaSelecionado = Enum.GetName(typeof(Semana), Convert.ToInt32(semana)).ToUpper() + "-FEIRA";
                    dados = dados.Where(_ => _.Dia_Sem == semanaSelecionado);
                }
                if (turnoPreenchido)
                {
                    var turnoSelecionado = Enum.GetName(typeof(Turno), Convert.ToInt32(turno)).ToUpper();
                    dados = dados.Where(_ => _.Noite_dia == turnoSelecionado);
                }
                if (tipoPreenchido)
                {
                    var tipoSelecionado = Enum.GetName(typeof(Tipo), Convert.ToInt32(tipo)).ToUpper();
                    dados = dados.Where(_ => _.Tipo_Acid == tipoSelecionado);
                }
                if (veiculoPreenchido)
                {
                    var veiculoSelecionado = Enum.GetName(typeof(Veiculo), Convert.ToInt32(veiculo));
                    if (veiculoSelecionado.Equals("Automovel")) dados = dados.Where(_ => _.Auto > 0);
                    else if (veiculoSelecionado.Equals("Taxi")) dados = dados.Where(_ => _.Taxi > 0);
                    else if (veiculoSelecionado.Equals("Lotacao")) dados = dados.Where(_ => _.Lotacao > 0);
                    else if (veiculoSelecionado.Equals("OnibusUrb")) dados = dados.Where(_ => _.Onibus_Urb > 0);
                    else if (veiculoSelecionado.Equals("OnibusMet")) dados = dados.Where(_ => _.Onibus_Met > 0);
                    else if (veiculoSelecionado.Equals("OnibusInt")) dados = dados.Where(_ => _.Onibus_Int > 0);
                    else if (veiculoSelecionado.Equals("Caminhao")) dados = dados.Where(_ => _.Caminhao > 0);
                    else if (veiculoSelecionado.Equals("Moto")) dados = dados.Where(_ => _.Moto > 0);
                    else if (veiculoSelecionado.Equals("Carroca")) dados = dados.Where(_ => _.Carroca > 0);
                    else if (veiculoSelecionado.Equals("Bicicleta")) dados = dados.Where(_ => _.Bicicleta > 0);
                    else if (veiculoSelecionado.Equals("Outro")) dados = dados.Where(_ => _.Outro > 0);
                }
                if (fatalPreenchido)
                {
                    dados = dados.Where(_ => _.Fatais > 0);
                }
                var dadosJson = dados.Select(_ => new { _.Latitude, _.Longitude }).ToList();
                var jsonResult = Json(new { Dados = dadosJson }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            #endregion
            #region SemNenhumAnoSelecionado
            var dadoss = db.Acidentes.Where(_ => _.Ano >= 2000 && _.Ano <= 2013);
            if (climaPreenchido)
            {
                var climaSelecionado = Enum.GetName(typeof(Clima), Convert.ToInt32(clima)).ToUpper();
                dadoss = dadoss.Where(_ => _.Tempo == climaSelecionado);
            }
            if (regiaoPreenchido)
            {
                var regiaoSelecionado = Enum.GetName(typeof(Regiao), Convert.ToInt32(regiao)).ToUpper();
                dadoss = dadoss.Where(_ => _.Regiao == regiaoSelecionado);
            }
            if (semanaPreenchido)
            {
                var semanaSelecionado = Enum.GetName(typeof(Semana), Convert.ToInt32(semana)).ToUpper() + "-FEIRA";
                dadoss = dadoss.Where(_ => _.Dia_Sem == semanaSelecionado);
            }
            if (turnoPreenchido)
            {
                var turnoSelecionado = Enum.GetName(typeof(Turno), Convert.ToInt32(turno)).ToUpper();
                dadoss = dadoss.Where(_ => _.Noite_dia == turnoSelecionado);
            }
            if (tipoPreenchido)
            {
                var tipoSelecionado = Enum.GetName(typeof(Tipo), Convert.ToInt32(tipo)).ToUpper();
                dadoss = dadoss.Where(_ => _.Tipo_Acid == tipoSelecionado);
            }
            if (veiculoPreenchido)
            {
                var veiculoSelecionado = Enum.GetName(typeof(Veiculo), Convert.ToInt32(veiculo));
                if (veiculoSelecionado.Equals("Automovel")) dadoss = dadoss.Where(_ => _.Auto > 0);
                else if (veiculoSelecionado.Equals("Taxi")) dadoss = dadoss.Where(_ => _.Taxi > 0);
                else if (veiculoSelecionado.Equals("Lotacao")) dadoss = dadoss.Where(_ => _.Lotacao > 0);
                else if (veiculoSelecionado.Equals("OnibusUrb")) dadoss = dadoss.Where(_ => _.Onibus_Urb > 0);
                else if (veiculoSelecionado.Equals("OnibusMet")) dadoss = dadoss.Where(_ => _.Onibus_Met > 0);
                else if (veiculoSelecionado.Equals("OnibusInt")) dadoss = dadoss.Where(_ => _.Onibus_Int > 0);
                else if (veiculoSelecionado.Equals("Caminhao")) dadoss = dadoss.Where(_ => _.Caminhao > 0);
                else if (veiculoSelecionado.Equals("Moto")) dadoss = dadoss.Where(_ => _.Moto > 0);
                else if (veiculoSelecionado.Equals("Carroca")) dadoss = dadoss.Where(_ => _.Carroca > 0);
                else if (veiculoSelecionado.Equals("Bicicleta")) dadoss = dadoss.Where(_ => _.Bicicleta > 0);
                else if (veiculoSelecionado.Equals("Outro")) dadoss = dadoss.Where(_ => _.Outro > 0);
            }
            if (fatalPreenchido)
            {
                dadoss = dadoss.Where(_ => _.Fatais > 0);
            }
            var dadossJson = dadoss.Select(_ => new { _.Latitude, _.Longitude }).ToList();
            var jsonnResult = Json(new { Dados = dadossJson }, JsonRequestBehavior.AllowGet);
            jsonnResult.MaxJsonLength = int.MaxValue;
            return jsonnResult;
            #endregion
            #endregion
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
