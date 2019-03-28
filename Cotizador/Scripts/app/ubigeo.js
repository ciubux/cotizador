function onChangeDepartamento(ddlDepartamento, ddlProvincia, ddlDistrito, divProvincias, divDistritos, urlProvinciasPorDepartamento, urlDistritosPorProvincia) {

    var departamentoSelector = "#" + ddlDepartamento;
    var provinciaSelector = "#" + ddlProvincia;
    var divProvinciasSelector = "#" + divProvincias;
    var divDistritosSelector = "#" + divDistritos;

    var codigoDepartamento = $(departamentoSelector).val();
    var codigoProvincia = $(provinciaSelector).val();

    $(divProvinciasSelector).load(urlProvinciasPorDepartamento + "?departamentoSelectId=" + ddlDepartamento + "&provinciaSelectId=" + ddlProvincia + "&distritoSelectId=" + ddlDistrito + "&distritoDivId=" + divDistritos + "&codigoDepartamento=" + codigoDepartamento);
    $(divDistritosSelector).load(urlDistritosPorProvincia + "?distritoSelectId=" + ddlDistrito + "&codigoDepartamento=" + codigoDepartamento + "&codigoProvincia=" + codigoProvincia);
}

function onChangeProvincia(ddlDepartamento, ddlProvincia, ddlDistrito, divDistritos, urlDistritosPorProvincia) {

    var departamentoSelector = "#" + ddlDepartamento;
    var provinciaSelector = "#" + ddlProvincia;
    var divDistritosSelector = "#" + divDistritos;

    var codigoDepartamento = $(departamentoSelector).val();
    var codigoProvincia = $(provinciaSelector).val();

    $(divDistritosSelector).load(urlDistritosPorProvincia + "?distritoSelectId=" + ddlDistrito + "&codigoDepartamento=" + codigoDepartamento + "&codigoProvincia=" + codigoProvincia);
}


