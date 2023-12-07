using Entidades.DataBase;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;


namespace Entidades.Modelos
{

    public delegate void DelegadoDemoraAtencion(double demora);
    public delegate void DelegadoNuevoIngreso(IComestible menu);
    


    public class Cocinero<T> where T : IComestible, new()
    {
        private int cantPedidosFinalizados;
        private string nombre;
        private double demoraPreparacionTotal;
        private CancellationTokenSource cancellation;
        private Task tarea;
        private T menu;
        public event DelegadoDemoraAtencion OnDemora;
        public event DelegadoNuevoIngreso OnIngreso;

        public Cocinero(string nombre)
        {
            this.nombre = nombre;
        }

        //No hacer nada
        public bool HabilitarCocina
        {
            get
            {
                return this.tarea is not null && (this.tarea.Status == TaskStatus.Running ||
                    this.tarea.Status == TaskStatus.WaitingToRun ||
                    this.tarea.Status == TaskStatus.WaitingForActivation);
            }
            set
            {
                if (value && !this.HabilitarCocina)
                {
                    this.cancellation = new CancellationTokenSource();
                    this.IniciarIngreso();
                }
                else
                {
                    this.cancellation.Cancel();
                }
            }
        }

        //no hacer nada
        public double TiempoMedioDePreparacion { get => this.cantPedidosFinalizados == 0 ? 0 : this.demoraPreparacionTotal / this.cantPedidosFinalizados; }
        public string Nombre { get => nombre; }
        public int CantPedidosFinalizados { get => cantPedidosFinalizados; }

        private void IniciarIngreso()
        {
            this.tarea = Task.Run(() => {

                while (!cancellation.IsCancellationRequested)
                {
                    NotificarNuevoIngreso();

                    EsperarProximoIngreso();

                    this.cantPedidosFinalizados++;

                    DataBaseManager.GuardarTicket(this.nombre,this.menu);
                }
            });   
        }

        private void NotificarNuevoIngreso()
        {
            if (OnIngreso != null)
            {
                this.menu = new T();

                this.menu.IniciarPreparacion();

                this.OnIngreso.Invoke(this.menu);
            }
        }

        private void EsperarProximoIngreso()
        {
            if(this.OnDemora != null )
            {
                int segundosTranscurridos = 0;

                while (!cancellation.Token.IsCancellationRequested && !menu.Estado)
                {
                    this.OnDemora.Invoke(segundosTranscurridos);

                    Thread.Sleep(1000);

                    segundosTranscurridos++;
                }

                this.demoraPreparacionTotal += segundosTranscurridos;
            }
        }
    }
}
