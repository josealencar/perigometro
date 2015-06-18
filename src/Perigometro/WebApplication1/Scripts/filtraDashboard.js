var myPieChart;

function inicializa() {
    var ctx = document.getElementById("myChart").getContext("2d");
    var dataGrafico = [
        {
            value: 21,
            color: "#F7464A",
            highlight: "#FF5A5E",
            label: "SUL"
        },
        {
            value: 20,
            color: "#46BFBD",
            highlight: "#5AD3D1",
            label: "NORTE"
        },
        {
            value: 18,
            color: "#FDB45C",
            highlight: "#FFC870",
            label: "LESTE"
        },
        {
            value: 4,
            color: "#27ae60",
            highlight: "#2ecc71",
            label: "CENTRO"
        }
    ];
    myPieChart = new Chart(ctx).Pie(dataGrafico, { animateScale: true });
};

function buscaEstatisticas(ano1, ano2)
{
    var anos = [];
    anos.push(ano1);
    anos.push(ano2);
    json = JSON.stringify({'Anos': anos});
    $.ajax({
        url: '/Dashboard/Estatisticas',
        data: json,
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
    }).done(function (res) {
        var mortes = res.Mortes;
        mortes.forEach(function (item, i) {
            myPieChart.segments[i].value = item.Qtde;
            myPieChart.segments[i].label = item.Regiao;
        });
        myPieChart.update();
    })
}