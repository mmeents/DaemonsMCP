
namespace DaemonsConfigViewer {
  internal static class Program {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() {
      try { 
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        ApplicationConfiguration.Initialize();
        var context = new DaemonsAppContext();
        var mainForm = context.CreateMainForm();
        Application.Run(mainForm);

      } catch (Exception ex) {        
        MessageBox.Show($"Fatal error starting application: {ex.Message}", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }
  }
}