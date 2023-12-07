using Entidades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Modelos
{
    public delegate void DelegadoNuevoPedido<T>(T menu);

    public class Mozo<T> where T : IComestible, new()
    {
        private CancellationTokenSource cancellation;
        private T menu;
        private Task tarea;

        public event DelegadoNuevoPedido<T> OnPedido;
         
        // Seguir despues 
        public bool EmpezarATrabajar {
            get
            {
                if (tarea != null && tarea.Status == TaskStatus.Running || tarea.Status == TaskStatus.WaitingToRun || tarea.Status == TaskStatus.WaitingForActivation)
                {
                    return true;
                }
                else return false;
            } 
            set
            {
               // estado no es Running o no es WaitingToRun o no es WaitingForActivation,

                if (value == true && tarea == null || tarea.Status != TaskStatus.Running || tarea.Status != TaskStatus.WaitingToRun || tarea.Status != TaskStatus.WaitingForActivation)
                {

                    cancellation = new CancellationTokenSource();
                    TomarPedidos();
                }
                else cancellation.Cancel();

            }
        }


        private void TomarPedidos()
        {
            while (!cancellation.IsCancellationRequested)
            {
                Thread.Sleep(50000);

                if (OnPedido != null)
                {
                    NotificarNuevoPedido();
                }
            }
        }


        private void NotificarNuevoPedido()
        {
            menu = new T();
            menu.IniciarPreparacion();
            OnPedido.Invoke(menu);
        }   


    }
}
