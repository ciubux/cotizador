﻿using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Cotizador.Models;
using BusinessLayer;
using Model;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace Cotizador.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()//string returnUrl)
        {
            //ViewBag.ReturnUrl = returnUrl;
            ViewBag.ReturnUrl = "Account/Login";
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // No cuenta los errores de inicio de sesión para el bloqueo de la cuenta
            // Para permitir que los errores de contraseña desencadenen el bloqueo de la cuenta, cambie a shouldLockout: true
            //    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            UsuarioBL usuarioBL = new UsuarioBL();
            Model.Usuario usuario = usuarioBL.getUsuarioLogin(model.Email, model.Password);
            this.Session["usuario"] = usuario;
            var result = usuario != null && usuario.idUsuario != null && usuario.idUsuario != Guid.Empty;
            SignInStatus signInStatus = SignInStatus.Failure;
            if (result)
                signInStatus = SignInStatus.Success;

            switch (signInStatus)
            {
                case SignInStatus.Success:

                    FamiliaBL familiaBL = new FamiliaBL();
                    List<Familia> familiaList = familiaBL.getFamilias();
                    this.Session["familiaList"] = familiaList;

                    ProveedorBL proveedorBL = new ProveedorBL();
                    List<Proveedor> proveedorList = proveedorBL.getProveedores();
                    this.Session["proveedorList"] = proveedorList;

                    CiudadBL ciudadBL = new CiudadBL();
                    List<Ciudad> ciudadList = ciudadBL.getCiudades();
                    usuario.sedesMP = ciudadList;

                    #region SedesCotizacion
                    /*Sedes para guias de remision*/
                    usuario.sedesMPCotizaciones = new List<Ciudad>();
                    /*Usuarios a su cargo*/
                    List<Usuario> usuarioCreaCotizacionList = new List<Usuario>();

                    if (usuario.apruebaCotizacionesLima)
                    {
                        foreach (Ciudad ciudad in ciudadList)
                        {
                            if (!ciudad.esProvincia)
                            {
                                usuario.sedesMPCotizaciones.Add(ciudad);
                            }
                        }
                     /*   foreach (Usuario usuarioTmp in usuario.usuarioCreaCotizacionList)
                        {
                            if (!usuarioTmp.sedeMP.esProvincia)
                            {
                                usuarioCreaCotizacionList.Add(usuarioTmp);
                            }
                        }*/
                    }

                    if (usuario.apruebaCotizacionesProvincias || usuario.creaCotizacionesProvincias)
                    {
                        foreach (Ciudad ciudad in ciudadList)
                        {
                            if (ciudad.esProvincia)
                            {
                                usuario.sedesMPCotizaciones.Add(ciudad);
                            }
                        }
                     /*   if (usuario.apruebaCotizacionesProvincias)
                        {
                            foreach (Usuario usuarioTmp in usuario.usuarioCreaCotizacionList)
                            {
                                if (usuarioTmp.sedeMP.esProvincia)
                                {
                                    usuarioCreaCotizacionList.Add(usuarioTmp);
                                }
                            }
                        }*/
                    }

                    


                    

                    if (!usuario.apruebaCotizaciones)
                    {
                        if (usuario.sedesMPCotizaciones.Where(s => s.idCiudad == usuario.sedeMP.idCiudad).FirstOrDefault() == null)
                        {
                            foreach (Ciudad ciudad in ciudadList)
                            {
                                if (ciudad.idCiudad == usuario.sedeMP.idCiudad)
                                {
                                    usuario.sedesMPCotizaciones.Add(ciudad);
                                    break;
                                }
                            }
                        }
                    }
              /*      else
                    {
                        usuario.usuarioCreaCotizacionList = usuarioCreaCotizacionList;
                    }*/

                    #endregion


                    #region SedesPedido

                    usuario.sedesMPPedidos = new List<Ciudad>();
                    /*Usuarios a su cargo*/
                    List<Usuario> usuarioTomaPedidoList = new List<Usuario>();

                    if (usuario.apruebaPedidosLima || usuario.visualizaPedidosLima)
                    {
                        foreach (Ciudad ciudad in ciudadList)
                        {
                            if (!ciudad.esProvincia)
                            {
                                usuario.sedesMPPedidos.Add(ciudad);
    
                            }
                        }
                   //     if (usuario.apruebaPedidosLima)
                        {
                            foreach (Usuario usuarioTmp in usuario.usuarioTomaPedidoList)
                            {
                                if (!usuarioTmp.sedeMP.esProvincia)
                                {
                                    usuarioTomaPedidoList.Add(usuarioTmp);
                                }
                            }
                        }

                    }

                    if (usuario.apruebaPedidosProvincias || usuario.tomaPedidosProvincias || usuario.visualizaPedidosProvincias)
                    {
                        foreach (Ciudad ciudad in ciudadList)
                        {
                            if (ciudad.esProvincia)
                            {
                                usuario.sedesMPPedidos.Add(ciudad);
                            }
                        }
                        if (usuario.apruebaPedidosProvincias)
                        {
                            foreach (Usuario usuarioTmp in usuario.usuarioTomaPedidoList)
                            {
                                if (usuarioTmp.sedeMP.esProvincia)
                                {
                                    usuarioTomaPedidoList.Add(usuarioTmp);
                                }
                            }
                        }
                    }



                    if (!usuario.apruebaPedidos)
                    {
                        if (usuario.sedesMPPedidos.Where(s => s.idCiudad == usuario.sedeMP.idCiudad).FirstOrDefault() == null)
                        {
                            foreach (Ciudad ciudad in ciudadList)
                            {
                                if (ciudad.idCiudad == usuario.sedeMP.idCiudad)
                                {
                                    usuario.sedesMPPedidos.Add(ciudad);
                                    break;
                                }
                            }
                        }
                    }
            //        else
                    {
                        usuario.usuarioTomaPedidoList = usuarioTomaPedidoList;
                    }



                    #endregion


              
                    #region SedesGuias

                    usuario.sedesMPGuiasRemision = new List<Ciudad>();
                    /*Usuarios a su cargo*/
                    List<Usuario> usuarioCreaGuiaList = new List<Usuario>();

                    if (usuario.administraGuiasLima)
                    {
                        foreach (Ciudad ciudad in ciudadList)
                        {
                            if (!ciudad.esProvincia)
                            {
                                usuario.sedesMPGuiasRemision.Add(ciudad);
                            }
                        }
                        foreach (Usuario usuarioTmp in usuario.usuarioCreaGuiaList)
                        {
                            if (!usuarioTmp.sedeMP.esProvincia)
                            {
                                usuarioCreaGuiaList.Add(usuarioTmp);
                            }
                        }

                    }

                    if (usuario.administraGuiasProvincias)
                    {
                        foreach (Ciudad ciudad in ciudadList)
                        {
                            if (ciudad.esProvincia)
                            {
                                usuario.sedesMPGuiasRemision.Add(ciudad);
                            }
                        }
                        foreach (Usuario usuarioTmp in usuario.usuarioCreaGuiaList)
                        {
                            if (usuarioTmp.sedeMP.esProvincia)
                            {
                                usuarioCreaGuiaList.Add(usuarioTmp);
                            }
                        }
                    }

                    if (!usuario.administraGuias)
                    {
                        foreach (Ciudad ciudad in ciudadList)
                        {
                            if (ciudad.idCiudad == usuario.sedeMP.idCiudad)
                            {
                                usuario.sedesMPGuiasRemision.Add(ciudad);
                                break;
                            }
                        }
                    }
                    else
                    {
                        usuario.usuarioCreaGuiaList = usuarioCreaGuiaList;
                    }

                    #endregion


                    #region SedesDocumentosVenta

                    usuario.sedesMPDocumentosVenta = new List<Ciudad>();
                    /*Usuarios a su cargo*/
                    List<Usuario> usuarioCreaDocumentoVentaList = new List<Usuario>();

                    if (usuario.administraDocumentosVentaLima)
                    {
                        foreach (Ciudad ciudad in ciudadList)
                        {
                            if (!ciudad.esProvincia)
                            {
                                usuario.sedesMPDocumentosVenta.Add(ciudad);
                            }
                        }
                        foreach (Usuario usuarioTmp in usuario.usuarioCreaDocumentoVentaList)
                        {
                            if (!usuarioTmp.sedeMP.esProvincia)
                            {
                                usuarioCreaDocumentoVentaList.Add(usuarioTmp);
                            }
                        }

                    }

                    if (usuario.administraDocumentosVentaProvincias)
                    {
                        foreach (Ciudad ciudad in ciudadList)
                        {
                            if (ciudad.esProvincia)
                            {
                                usuario.sedesMPDocumentosVenta.Add(ciudad);
                            }
                        }
                        foreach (Usuario usuarioTmp in usuario.usuarioCreaDocumentoVentaList)
                        {
                            if (usuarioTmp.sedeMP.esProvincia)
                            {
                                usuarioCreaDocumentoVentaList.Add(usuarioTmp);
                            }
                        }
                    }

                    if (!usuario.administraDocumentosVenta)
                    {
                        foreach (Ciudad ciudad in ciudadList)
                        {
                            if (ciudad.idCiudad == usuario.sedeMP.idCiudad)
                            {
                                usuario.sedesMPDocumentosVenta.Add(ciudad);
                                break;
                            }
                        }
                    }
                    else
                    {
                        usuario.usuarioCreaDocumentoVentaList = usuarioCreaDocumentoVentaList;
                    }

                    #endregion

                    try
                    {
                        if (usuario.cotizacionSerializada != null)
                        {
                            Cotizacion cotizacion = JsonConvert.DeserializeObject<Cotizacion>(usuario.cotizacionSerializada);
                            //usuario.cotizacionSerializada = null;
                            this.Session[Constantes.VAR_SESSION_USUARIO] = usuario;
                            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoCotizacion;
                            this.Session[Constantes.VAR_SESSION_COTIZACION] = cotizacion;
                        }
                    }
                    catch (Exception e)
                    {
                        this.Session[Constantes.VAR_SESSION_COTIZACION] = null;
                    }

                    try
                    {   if (usuario.pedidoSerializado != null)
                        {
                            Pedido pedido = JsonConvert.DeserializeObject<Pedido>(usuario.pedidoSerializado);
                           
                            //usuario.pedidoSerializado = null;
                            this.Session[Constantes.VAR_SESSION_USUARIO] = usuario;
                            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoPedido;
                            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
                        }
                    }
                    catch (Exception e)
                    {
                        this.Session[Constantes.VAR_SESSION_PEDIDO] = null;
                    }

                    if (this.Session["Prev_Request_Url"] != null)
                    {
                        string prevUrl = this.Session["Prev_Request_Url"].ToString();
                        this.Session["Prev_Request_Url"] = null;
                        return Redirect(prevUrl);
                    }

                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Intento de inicio de sesión no válido.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Requerir que el usuario haya iniciado sesión con nombre de usuario y contraseña o inicio de sesión externo
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // El código siguiente protege de los ataques por fuerza bruta a los códigos de dos factores. 
            // Si un usuario introduce códigos incorrectos durante un intervalo especificado de tiempo, la cuenta del usuario 
            // se bloqueará durante un período de tiempo especificado. 
            // Puede configurar el bloqueo de la cuenta en IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Código no válido.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // Para obtener más información sobre cómo habilitar la confirmación de cuenta y el restablecimiento de contraseña, visite http://go.microsoft.com/fwlink/?LinkID=320771
                    // Enviar correo electrónico con este vínculo
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", "Para confirmar la cuenta, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");
                   
                }
                AddErrors(result);
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // No revelar que el usuario no existe o que no está confirmado
                    return View("ForgotPasswordConfirmation");
                }

                // Para obtener más información sobre cómo habilitar la confirmación de cuenta y el restablecimiento de contraseña, visite http://go.microsoft.com/fwlink/?LinkID=320771
                // Enviar correo electrónico con este vínculo
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Restablecer contraseña", "Para restablecer la contraseña, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // No revelar que el usuario no existe
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Solicitar redireccionamiento al proveedor de inicio de sesión externo
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generar el token y enviarlo
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Si el usuario ya tiene un inicio de sesión, iniciar sesión del usuario con este proveedor de inicio de sesión externo
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Si el usuario no tiene ninguna cuenta, solicitar que cree una
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Obtener datos del usuario del proveedor de inicio de sesión externo
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpGet]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            this.Session["Usuario"] = null;
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "General");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Aplicaciones auxiliares
        // Se usa para la protección XSRF al agregar inicios de sesión externos
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            /*  if (Url.IsLocalUrl(returnUrl))
              {
                  return Redirect(returnUrl);
              }*/

        /*    if (this.Session[Constantes.VAR_SESSION_PAGINA] != null)
            {
                int pagina = (int)this.Session[Constantes.VAR_SESSION_PAGINA];
                if (pagina == (int)Constantes.paginas.MantenimientoCotizacion)
                {
                    return RedirectToAction("Cotizar", "Cotizacion");
                }
                else if (pagina == (int)Constantes.paginas.MantenimientoPedido)
                {
                    return RedirectToAction("Pedir", "Pedido");
                }
            }*/


            Usuario usuario = (Model.Usuario)this.Session["usuario"];
            if (usuario != null)
            {

                /*    if (usuario.tomaPedidos)
                    return RedirectToAction("Pedir", "Pedido");*/
                if (usuario.apruebaPedidos || usuario.tomaPedidos || usuario.visualizaPedidos)
                    return RedirectToAction("Index", "Pedido");
                /*   if(usuario.creaCotizaciones)
                       return RedirectToAction("Cotizar", "Cotizacion");*/
                if (usuario.creaCotizaciones || usuario.apruebaCotizaciones || usuario.visualizaCotizaciones)
                    return RedirectToAction("Index", "Cotizacion");            

                if (usuario.creaGuias || usuario.visualizaGuias)
                    return RedirectToAction("Index", "GuiaRemision");

                if (usuario.creaDocumentosVenta || usuario.visualizaDocumentosVenta)
                    return RedirectToAction("Index", "Factura");

                if (usuario.modificaMaestroClientes)
                    return RedirectToAction("Edita", "Cliente");
            }

            return RedirectToAction("Index", "Cotizacion");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}