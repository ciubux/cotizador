/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */


jQuery(function($){
    $.datepicker.regional['es'] = {
        closeText: 'Cerrar',
        prevText: '< Ant',
        nextText: 'Sig >',
        currentText: 'Hoy',
        monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
        monthNamesShort: ['Ene','Feb','Mar','Abr', 'May','Jun','Jul','Ago','Sep', 'Oct','Nov','Dic'],
        dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
        dayNamesShort: ['Dom','Lun','Mar','Mié','Juv','Vie','Sáb'],
        dayNamesMin: ['Do','Lu','Ma','Mi','Ju','Vi','Sá'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
        };
    $.datepicker.setDefaults($.datepicker.regional['es']);
    
    
    $('.table').footable();
    
    $("#fecha").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", new Date());

    $("#categoria").change(function () {
        //alert(1);
        $.ajax({
            url: "/Home/GetFamilias",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCategoria: $(this).val(),
            },
            success: function (res) {
                var selects = '<option selected>Seleccione...</option>';

                res.results.forEach(function (item, index, array) {
                    selects = selects + '<option value="' + item.id + '" >' + item.text + '</option>';
                });

                $('#familia').html(selects);
            }
        });
    });
});