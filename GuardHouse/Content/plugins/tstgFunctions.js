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

function exportToCsv(filename, rows) {

    filename = filename.replace(" ", "");

    var json = rows;//json3.items
    var fields = Object.keys(json[0])
    var replacer = function (key, value) { return value === null ? '' : value }
    var csv = json.map(function (row) {
        return fields.map(function (fieldName) {
            return JSON.stringify(row[fieldName], replacer)
        }).join(',')
    })
    csv.unshift(fields.join(',')) // add header column
    
    var blob = new Blob([csv.join('\r\n')], { type: 'text/csv;charset=utf-8;' });
    if (navigator.msSaveBlob) { // IE 10+
        navigator.msSaveBlob(blob, filename);
    } else {
        var link = document.createElement("a");
        if (link.download !== undefined) { // feature detection
            // Browsers that support HTML5 download attribute
            var url = URL.createObjectURL(blob);
            link.setAttribute("href", url);
            link.setAttribute("download", filename);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    }
}