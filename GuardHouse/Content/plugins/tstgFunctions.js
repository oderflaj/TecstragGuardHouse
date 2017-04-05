function myTrim(x) {
    return x.replace(/^\s+|\s+$/gm, '');
}
function SetSelectDT(parTable) {
    var tr = $('#' + parTable + ' tbody tr');

    tr.map((i, t) => {
        t.style.cursor = "pointer";
    });

    $('#' + parTable + ' tbody').on('click', 'tr', function () {
        var tr = $('#' + parTable + ' tbody tr');

        tr.map((i, t) => {
            t.style.cursor = "pointer";
        });
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
            $(".selectedBtn").attr("disabled", "disabled");
            $(".selectedBtn").addClass("disabled");
        }
        else {
            $('#' + parTable + ' tbody tr.selected').removeClass('selected');
            $(this).addClass('selected');
            $(".selectedBtn").removeAttr("disabled");
            $(".selectedBtn").removeClass("disabled");
        }
    });
}

function SetSelectDTActive(parTable, parAction) {

    var tr = $('#' + parTable + ' tbody tr');

    tr.map((i, t) => {
        t.style.cursor = "pointer";
    });

    $('#' + parTable + ' tbody').on('click', 'tr', function () {
        var tr = $('#' + parTable + ' tbody tr');

        tr.map((i, t) => {
            t.style.cursor = "pointer";
        });
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
            
            $(".selectedBtn").attr("disabled", "disabled");
            $(".selectedBtn").addClass("disabled");
        }
        else {
            $('#' + parTable + ' tbody tr.selected').removeClass('selected');
            $(this).addClass('selected');
            $(this).css("cursor", "");
            $(".selectedBtn").removeAttr("disabled");
            $(".selectedBtn").removeClass("disabled");

            $(".selected").bind("click", parAction($(".selected")[0].cells))
        }
        
    });
}