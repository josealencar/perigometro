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
            var anoInicial = Request.Form.Get("AnoInicial");
            var anoFinal = Request.Form.Get("Ano");
            var clima = Request.Form.Get("Clima");
            var regiao = Request.Form.Get("Regiao");
            var semana = Request.Form.Get("Semana");
            var turno = Request.Form.Get("Turno");
            var tipo = Request.Form.Get("Tipo");
            var veiculo = Request.Form.Get("Veiculo");
            var fatal = Request.Form.Get("Fatal");

            #region booleanos
            bool anoInicialPreenchido = anoInicial != null;
            bool anoFinalPreenchido = anoFinal != null;
            bool climaPreenchido = clima != null;
            bool regiaoPreenchido = regiao != null;
            bool semanaPreenchido = semana != null;
            bool turnoPreenchido = turno != null;
            bool tipoPreenchido = tipo != null;
            bool veiculoPreenchido = veiculo!= null;
            bool fatalPreenchido = fatal != null;
            #endregion

            #region FormComTodosOsCamposPreenchidos
            if (anoInicialPreenchido && anoFinalPreenchido && climaPreenchido && regiaoPreenchido && semanaPreenchido
                && turnoPreenchido && tipoPreenchido && veiculoPreenchido && fatalPreenchido)
            {
                var anoInicialSelecionado = RetornaAnoCorreto(Convert.ToInt32(anoInicial));
                var anoFinalSelecionado = RetornaAnoCorreto(Convert.ToInt32(anoFinal));
                var climaSelecionado = Enum.GetName(typeof(Clima), Convert.ToInt32(clima)).ToUpper();
                var regiaoSelecionado = Enum.GetName(typeof(Regiao), Convert.ToInt32(regiao)).ToUpper();
                var semanaSelecionado = Enum.GetName(typeof(Semana), Convert.ToInt32(semana)).ToUpper() + "-FEIRA";
                var turnoSelecionado = Enum.GetName(typeof(Turno), Convert.ToInt32(turno)).ToUpper();
                var tipoSelecionado = Enum.GetName(typeof(Tipo), Convert.ToInt32(tipo)).ToUpper();
                var veiculoSelecionado = Enum.GetName(typeof(Veiculo), Convert.ToInt32(veiculo));
                #region AutomovelComFatais
                if (veiculoSelecionado.Equals("Automovel"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado && 
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Auto > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Auto > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado && 
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Auto > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region TaxiComFatais
                if (veiculoSelecionado.Equals("Taxi"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Taxi > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Taxi > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Taxi > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region LotacaoComFatais
                if (veiculoSelecionado.Equals("Lotacao"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Lotacao > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Lotacao > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Lotacao > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OnibusUrbComFatais
                if (veiculoSelecionado.Equals("OnibusUrb"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Onibus_Urb > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Onibus_Urb > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Onibus_Urb > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OnibusMetComFatais
                if (veiculoSelecionado.Equals("OnibusMet"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Onibus_Met > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Onibus_Met > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Onibus_Met > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OnibusIntComFatais
                if (veiculoSelecionado.Equals("OnibusInt"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Onibus_Int > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Onibus_Int > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Onibus_Int > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region CaminhaoComFatais
                if (veiculoSelecionado.Equals("Caminhao"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Caminhao > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Caminhao > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Caminhao > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region MotoComFatais
                if (veiculoSelecionado.Equals("Moto"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Moto > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Moto > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Moto > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region CarrocaComFatais
                if (veiculoSelecionado.Equals("Carroca"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Carroca > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Carroca > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Carroca > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region BicicletaComFatais
                if (veiculoSelecionado.Equals("Bicicleta"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Bicicleta > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Bicicleta > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Bicicleta > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OutroComFatais
                if (veiculoSelecionado.Equals("Outro"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Outro > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Outro > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Outro > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
            }
            #endregion
            #region FormComDemaisCamposPreenchidosExcetoFatais
            if (anoInicialPreenchido && anoFinalPreenchido && climaPreenchido && regiaoPreenchido && semanaPreenchido
                && turnoPreenchido && tipoPreenchido && veiculoPreenchido)
            {
                var anoInicialSelecionado = RetornaAnoCorreto(Convert.ToInt32(anoInicial));
                var anoFinalSelecionado = RetornaAnoCorreto(Convert.ToInt32(anoFinal));
                var climaSelecionado = Enum.GetName(typeof(Clima), Convert.ToInt32(clima)).ToUpper();
                var regiaoSelecionado = Enum.GetName(typeof(Regiao), Convert.ToInt32(regiao)).ToUpper();
                var semanaSelecionado = Enum.GetName(typeof(Semana), Convert.ToInt32(semana)).ToUpper() + "-FEIRA";
                var turnoSelecionado = Enum.GetName(typeof(Turno), Convert.ToInt32(turno)).ToUpper();
                var tipoSelecionado = Enum.GetName(typeof(Tipo), Convert.ToInt32(tipo)).ToUpper();
                var veiculoSelecionado = Enum.GetName(typeof(Veiculo), Convert.ToInt32(veiculo));
                #region AutomovelSemFatais
                if (veiculoSelecionado.Equals("Automovel"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Auto > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Auto > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Auto > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region TaxiSemFatais
                if (veiculoSelecionado.Equals("Taxi"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Taxi > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Taxi > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Taxi > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region LotacaoSemFatais
                if (veiculoSelecionado.Equals("Lotacao"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Lotacao > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Lotacao > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Lotacao > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OnibusUrbSemFatais
                if (veiculoSelecionado.Equals("OnibusUrb"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Onibus_Urb > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Onibus_Urb > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Onibus_Urb > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OnibusMetSemFatais
                if (veiculoSelecionado.Equals("OnibusMet"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Onibus_Met > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Onibus_Met > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Onibus_Met > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OnibusIntSemFatais
                if (veiculoSelecionado.Equals("OnibusInt"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Onibus_Int > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Onibus_Int > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Onibus_Int > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region CaminhaoSemFatais
                if (veiculoSelecionado.Equals("Caminhao"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Caminhao > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Caminhao > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Caminhao > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region MotoSemFatais
                if (veiculoSelecionado.Equals("Moto"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Moto > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Moto > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Moto > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region CarrocaSemFatais
                if (veiculoSelecionado.Equals("Carroca"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Carroca > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Carroca > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Carroca > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region BicicletaSemFatais
                if (veiculoSelecionado.Equals("Bicicleta"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Bicicleta > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Bicicleta > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Bicicleta > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OutroSemFatais
                if (veiculoSelecionado.Equals("Outro"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Outro > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado && _.Tipo_Acid == tipoSelecionado
                            && _.Outro > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Tipo_Acid == tipoSelecionado && _.Outro > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
            }
            #endregion

            #region FormSemTipoPreenchidoComFatal
            if (anoInicialPreenchido && anoFinalPreenchido && climaPreenchido && regiaoPreenchido && semanaPreenchido
                && turnoPreenchido && veiculoPreenchido && fatalPreenchido)
            {
                var anoInicialSelecionado = RetornaAnoCorreto(Convert.ToInt32(anoInicial));
                var anoFinalSelecionado = RetornaAnoCorreto(Convert.ToInt32(anoFinal));
                var climaSelecionado = Enum.GetName(typeof(Clima), Convert.ToInt32(clima)).ToUpper();
                var regiaoSelecionado = Enum.GetName(typeof(Regiao), Convert.ToInt32(regiao)).ToUpper();
                var semanaSelecionado = Enum.GetName(typeof(Semana), Convert.ToInt32(semana)).ToUpper() + "-FEIRA";
                var turnoSelecionado = Enum.GetName(typeof(Turno), Convert.ToInt32(turno)).ToUpper();
                var veiculoSelecionado = Enum.GetName(typeof(Veiculo), Convert.ToInt32(veiculo));
                #region AutomovelComFatais
                if (veiculoSelecionado.Equals("Automovel"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Auto > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Auto > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Auto > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region TaxiComFatais
                if (veiculoSelecionado.Equals("Taxi"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Taxi > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Taxi > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Taxi > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region LotacaoComFatais
                if (veiculoSelecionado.Equals("Lotacao"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Lotacao > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Lotacao > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Lotacao > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OnibusUrbComFatais
                if (veiculoSelecionado.Equals("OnibusUrb"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Urb > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Urb > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Urb > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OnibusMetComFatais
                if (veiculoSelecionado.Equals("OnibusMet"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Met > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Met > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Met > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OnibusIntComFatais
                if (veiculoSelecionado.Equals("OnibusInt"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Int > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Int > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Int > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region CaminhaoComFatais
                if (veiculoSelecionado.Equals("Caminhao"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Caminhao > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Caminhao > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Caminhao > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region MotoComFatais
                if (veiculoSelecionado.Equals("Moto"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Moto > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Moto > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Moto > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region CarrocaComFatais
                if (veiculoSelecionado.Equals("Carroca"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Carroca > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Carroca > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Carroca > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region BicicletaComFatais
                if (veiculoSelecionado.Equals("Bicicleta"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Bicicleta > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Bicicleta > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Bicicleta > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OutroComFatais
                if (veiculoSelecionado.Equals("Outro"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Outro > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Outro > 0 && _.Fatais > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Outro > 0 && _.Fatais > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
            }
            #endregion
            #region FormSemTipoPreenchidoESemFatais
            if (anoInicialPreenchido && anoFinalPreenchido && climaPreenchido && regiaoPreenchido && semanaPreenchido
                && turnoPreenchido && tipoPreenchido && veiculoPreenchido)
            {
                var anoInicialSelecionado = RetornaAnoCorreto(Convert.ToInt32(anoInicial));
                var anoFinalSelecionado = RetornaAnoCorreto(Convert.ToInt32(anoFinal));
                var climaSelecionado = Enum.GetName(typeof(Clima), Convert.ToInt32(clima)).ToUpper();
                var regiaoSelecionado = Enum.GetName(typeof(Regiao), Convert.ToInt32(regiao)).ToUpper();
                var semanaSelecionado = Enum.GetName(typeof(Semana), Convert.ToInt32(semana)).ToUpper() + "-FEIRA";
                var turnoSelecionado = Enum.GetName(typeof(Turno), Convert.ToInt32(turno)).ToUpper();
                var veiculoSelecionado = Enum.GetName(typeof(Veiculo), Convert.ToInt32(veiculo));
                #region AutomovelSemFatais
                if (veiculoSelecionado.Equals("Automovel"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Auto > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Auto > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Auto > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region TaxiSemFatais
                if (veiculoSelecionado.Equals("Taxi"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Taxi > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Taxi > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Taxi > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region LotacaoSemFatais
                if (veiculoSelecionado.Equals("Lotacao"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Lotacao > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Lotacao > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Lotacao > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OnibusUrbSemFatais
                if (veiculoSelecionado.Equals("OnibusUrb"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Urb > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Urb > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Urb > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OnibusMetSemFatais
                if (veiculoSelecionado.Equals("OnibusMet"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Met > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Met > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Met > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OnibusIntSemFatais
                if (veiculoSelecionado.Equals("OnibusInt"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Int > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Int > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Onibus_Int > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region CaminhaoSemFatais
                if (veiculoSelecionado.Equals("Caminhao"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Caminhao > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Caminhao > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Caminhao > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region MotoSemFatais
                if (veiculoSelecionado.Equals("Moto"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Moto > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Moto > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Moto > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region CarrocaSemFatais
                if (veiculoSelecionado.Equals("Carroca"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Carroca > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Carroca > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Carroca > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region BicicletaSemFatais
                if (veiculoSelecionado.Equals("Bicicleta"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Bicicleta > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Bicicleta > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Bicicleta > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
                #region OutroSemFatais
                if (veiculoSelecionado.Equals("Outro"))
                {
                    if (anoInicialSelecionado < anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoInicialSelecionado &&
                            _.Ano <= anoFinalSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Outro > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado > anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano >= anoFinalSelecionado &&
                            _.Ano <= anoInicialSelecionado && _.Tempo == climaSelecionado && _.Regiao == regiaoSelecionado
                            && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Outro > 0).Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                    if (anoInicialSelecionado == anoFinalSelecionado)
                    {
                        var dados = db.Acidentes.Where(_ => _.Ano == anoFinalSelecionado && _.Tempo == climaSelecionado &&
                            _.Regiao == regiaoSelecionado && _.Dia_Sem == semanaSelecionado && _.Noite_dia == turnoSelecionado
                            && _.Outro > 0)
                            .Select(_ => new { _.Latitude, _.Longitude }).ToList();
                        var jsonResultAutoFatais = Json(new { Dados = dados }, JsonRequestBehavior.AllowGet);
                        jsonResultAutoFatais.MaxJsonLength = int.MaxValue;
                        return jsonResultAutoFatais;
                    }
                }
                #endregion
            }
            #endregion

            var dadossss = db.Acidentes.Where(_ => _.Ano == 2000).Select(_ => new { _.Latitude, _.Longitude }).ToList();
            var jsonResult = Json(new { Dados = dadossss }, JsonRequestBehavior.AllowGet);
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

        private int RetornaAnoCorreto(int ano)
        {
            if (ano == 0) return 2000;
            if (ano == 1) return 2001;
            if (ano == 2) return 2002;
            if (ano == 3) return 2003;
            if (ano == 4) return 2004;
            if (ano == 5) return 2005;
            if (ano == 6) return 2006;
            if (ano == 7) return 2007;
            if (ano == 8) return 2008;
            if (ano == 9) return 2009;
            if (ano == 10) return 2010;
            if (ano == 11) return 2011;
            if (ano == 12) return 2012;
            return 2013;
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
