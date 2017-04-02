


$(document).ready(function () {

   /*
    $("#idCliente").on("change", function (e) {
        $("#idCliente option:selected").prop("selected", false)
    });*/ 
    $("#idCliente").chosen({ placeholder_text_single: "Seleccione el Cliente", no_results_text: "No existen coincidencias" });

    $("#idCliente").ajaxChosen({
        dataType: "json",
        type: "GET",
        minTermLength: 3,
        afterTypeDelay: 300,
        cache: false,
        url: URL_BASE + "HomeController/GetClientes"
    }, {
        loadingImg: URL_BASE + "Content/images/loading.gif"
    }, { placeholder_text_single: "Seleccione la Enfermedad", no_results_text: "No existen coincidencias" });




})