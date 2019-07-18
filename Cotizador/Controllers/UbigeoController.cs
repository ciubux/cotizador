using BusinessLayer;
using Cotizador.Models;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class UbigeoController : Controller
    {
        // GET: Ubigeo
        private readonly UbigeoBL _ubigeoBl = new UbigeoBL();

        public ActionResult GetDepartamentos(string departamentoSelectId, string provinciaSelectId, string distritoSelectId, string provinciaDivId, string distritoDivId, string selectedValue = null, Boolean disabled = false)
        {
            var model = new DepartamentoViewModels
            {
                Data = _ubigeoBl.GetDepartamentos(),
                DepartamentoSelectId = departamentoSelectId,
                ProvinciaSelectId = provinciaSelectId,
                DistritoSelectId = distritoSelectId,
                ProvinciaDivId = provinciaDivId,
                DistritoDivId = distritoDivId,
                UrlProvinciasPorDepartamento = Url.Action("GetProvinciasPorDepartamento"),
                UrlDistritosPorProvincia = Url.Action("GetDistritosPorProvincia"),
                Disabled = disabled,
                SelectedValue = selectedValue
            };

            return PartialView("_Departamento", model);
        }

        public ActionResult GetProvinciasPorDepartamento(string departamentoSelectId, string provinciaSelectId, string distritoSelectId, string distritoDivId, string codigoDepartamento = "01", string selectedValue = null, Boolean disabled = false)
        {
            var model = new ProvinciaViewModels
            {
                Data = _ubigeoBl.GetProvincias(codigoDepartamento),
                DepartamentoSelectId = departamentoSelectId,
                ProvinciaSelectId = provinciaSelectId,
                DistritoSelectId = distritoSelectId,
                DistritoDivId = distritoDivId,
                UrlDistritosPorProvincia = Url.Action("GetDistritosPorProvincia"),
                Disabled = disabled,
                SelectedValue = selectedValue
            };

            return PartialView("_Provincia", model);
        }

        public ActionResult GetDistritosPorProvincia(string distritoSelectId, string codigoDepartamento = "01", string codigoProvincia = "01", string selectedValue = null, Boolean disabled = false)
        {
            var model = new DistritoViewModels
            {
                Data = _ubigeoBl.GetDistritos(codigoDepartamento, codigoProvincia),
                DistritoSelectId = distritoSelectId,
                Disabled = disabled,
                SelectedValue = selectedValue
            };

            return PartialView("_Distrito", model);
        }
    }
}