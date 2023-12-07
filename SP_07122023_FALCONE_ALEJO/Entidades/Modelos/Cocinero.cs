using Entidades.DataBase;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;


namespace Entidades.Modelos
{
    public delegate void DelegadoDemoraAtencion(double demora);
    public delegate void DelegadoPedidoEnCurso(IComestible menu);

    public class Cocinero<T> where T : IComestible, new()
    {

        private CancellationTokenSource cancellation;
        private int cantPedidosFinalizados;
        private double demoraPreparacionTotal;
        private Mozo<T> mozo;    
        private string nombre;
        private Queue<T> pedidos;
        private T PedidoEnPrepaacion;
        public event DelegadoDemoraAtencion OnDemora;
        public event DelegadoPedidoEnCurso OnPedido;
        private Task tarea;

        public Cocinero(string nombre)
        {
            this.nombre = nombre;
            this.mozo = new Mozo<T>();
            this.pedidos = new Queue<T>();
            
        }

        public Queue<T> Pedidos { get { return this.pedidos; } }
        
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
                    this.mozo.EmpezarATrabajar = true;
                    this.EmpezarACocinar();
                }
                else
                {
                    this.cancellation.Cancel();
                    this.mozo.EmpezarATrabajar = false;
                }
            }
        }

        //no hacer nada
        public double TiempoMedioDePreparacion { get => this.cantPedidosFinalizados == 0 ? 0 : this.demoraPreparacionTotal / this.cantPedidosFinalizados; }
        public string Nombre { get => nombre; }
        public int CantPedidosFinalizados { get => cantPedidosFinalizados; }


        private void TomarNuevoPedido(T menu)
        {
            if (this.OnPedido != null)
            {
                this.pedidos.Enqueue(menu); 
            }
        }

        private void EmpezarACocinar()
        {
            this.tarea = Task.Run(() => {

                while (!cancellation.IsCancellationRequested)
                {
                    if (OnPedido != null && this.pedidos.Count > 0)
                    {
                        this.PedidoEnPrepaacion = this.pedidos.Dequeue();

                        this.EsperarProximoIngreso();

                        this.cantPedidosFinalizados++;

                        DataBaseManager.GuardarTicket(this.nombre,this.PedidoEnPrepaacion);
                    }
                }
            });   
        }

        private void EsperarProximoIngreso()
        {
            if(this.OnDemora != null )
            {
                int segundosTranscurridos = 0;

                while (!cancellation.IsCancellationRequested && !PedidoEnPrepaacion.Estado)
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
