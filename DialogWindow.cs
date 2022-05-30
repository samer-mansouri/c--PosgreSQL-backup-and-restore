using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace dba
{
    class DialogWindow : Dialog
    {
        [UI] private Label msgType;
        [UI] private Label textMsg;
        [UI] private Button closeTab;
        
        public DialogWindow(string msg, string text, string title) : this(new Builder("DialogWindow.glade"), msg, text, title) { }
        
        
        private DialogWindow(Builder builder, string msg, string text, string title) : base(builder.GetObject("DialogWindow").Handle)
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;
            this.Title = title;
            msgType.Text = msg;
            textMsg.Text = text;
            closeTab.Clicked += CloseTab;

        }
        
        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Hide();
        }

        private void CloseTab(object sender, EventArgs a)
        {
            this.Hide();
        }

    }
    

}