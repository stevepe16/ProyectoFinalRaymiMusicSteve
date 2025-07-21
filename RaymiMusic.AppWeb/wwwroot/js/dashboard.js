document.addEventListener("DOMContentLoaded", function () {
    // Chart: Duración de Canciones
    const duracionesCtx = document.getElementById('cancionesChart')?.getContext('2d');
    if (duracionesCtx && window.duracionLabels && window.duracionData) {
        new Chart(duracionesCtx, {
            type: 'bar',
            data: {
                labels: window.duracionLabels,
                datasets: [{
                    label: 'Duración (segundos)',
                    data: window.duracionData,
                    backgroundColor: 'rgba(54, 162, 235, 0.3)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                indexAxis: 'y',
                responsive: true,
                plugins: {
                    legend: { display: false }
                },
                scales: {
                    x: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Segundos', 
                            font: {
                                size: 14,
                                weight: 'bold'
                            }
                        }
                    }
                }
            }
        });
    }


    // Chart: Totales
    const totalesCtx = document.getElementById('totalesChart')?.getContext('2d');
    if (totalesCtx && window.totalData) {
        new Chart(totalesCtx, {
            type: 'bar',
            data: {
                labels: ['Canciones', 'Álbumes'],
                datasets: [{
                    data: window.totalData,
                    backgroundColor: ['#aaf', '#faa'],
                    borderColor: ['#88f', '#f88'],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                plugins: { legend: { display: false } },
                scales: { y: { beginAtZero: true } }
            }
        });
    }

    // Chart: Reproducciones
    const repsCtx = document.getElementById('reproduccionesChart')?.getContext('2d');
    if (repsCtx && window.repsLabels && window.repsData) {
        new Chart(repsCtx, {
            type: 'pie',
            data: {
                labels: window.repsLabels,
                datasets: [{
                    label: 'Reproducciones',
                    data: window.repsData,
                    backgroundColor: [
                        '#FF6384', '#36A2EB', '#FFCE56',
                        '#4BC0C0', '#9966FF', '#FF9F40'
                    ],
                    borderColor: 'white',
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { position: 'bottom' },
                    tooltip: {
                        callbacks: {
                            label: function (ctx) {
                                return `${ctx.label}: ${ctx.raw} reproducciones`;
                            }
                        }
                    }
                }
            }
        });
    }
});
