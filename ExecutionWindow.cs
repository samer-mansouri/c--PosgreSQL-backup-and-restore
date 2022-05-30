using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace dba
{
    public class ExecutionWindow : Window
    {

        public ExecutionWindow() : this(new Builder("ExecutionWindow.glade")) { }
        
        
        private ExecutionWindow(Builder builder) : base(builder.GetObject("ExecutionWindow").Handle)
        {
            builder.Autoconnect(this);

           
            this.Title = "Tâche en cours d'exécution";
            

        }
        
        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Hide();
        }

      
        
    }
}