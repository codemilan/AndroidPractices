/* 
	Now let’s jump back to the Android side of things and look at AsyncTask. This is a class in the Android framework for cleanly wrapping up asynchronous
	operations, and gives you hooks into the lifecycle for updating the UI, reporting progress, etc. The big win with this class is that you don’t need to 
	worry about explicitly calling things on the UI thread since each of the callbacks designed for updating the UI are already running on it. DoInBackground,
	the method that will contain the actual work you’re trying to do, stays on the background thread and you can publish progress updates from it. This approach
	does expose one typical pain point you’ll run into when porting Java code over to C#: unfortunately C# doesn’t support anonymous classes, which are used
	heavily in Android. Instead, you’ll typically need to pass in anything your task will need via the constructor. In this case the task needs the LoginService
	as well as a context to use when showing a progress dialog.
*/

using Android.App;
using Android.OS;
using Android.Content;

namespace TinyThreading
{

	public class LoginTask : AsyncTask
	{
		private ProgressDialog _progressDialog;
		private LoginService _loginService;
		private Context _context;

		public LoginTask(Context context, LoginService loginService)
		{
			_context = context;
			_loginService = loginService;
		}

		protected override void OnPreExecute()
		{
			base.OnPreExecute();

			_progressDialog = ProgressDialog.Show(_context, "Login In Progress", "Please wait...");
		}

		protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
		{
			_loginService.Login(@params[0].ToString());

			return true;
		}

		protected override void OnPostExecute(Java.Lang.Object result)
		{
			base.OnPostExecute(result);

			_progressDialog.Hide();

			new AlertDialog.Builder(_context)
				.SetTitle("Login Successful")
				.SetMessage("Great success!")
				.Show();
		}
	}
}
/* 
 OnPreExecute() gets called when the task fires up, but before it starts doing the work. Since it executes right on the UI thread, we use it to bring up a progress
 dialog. DoInBackground() does the real work of the task on a background thread, then once it returns we get the OnPostExecute() callback on the UI thread, where
 we hide the progress dialog and show the completion message.
To use the task just create a new instance of it, and call its Execute() method.

Here we have outlined several possible routes to go down when trying to do work in a background thread within your application. Each has its own set of advantages
and disadvantages, so in general I’d say to go with whichever seems like the right tool for the job. I tend to lean towards ThreadPool because it works across
platforms, allowing for more code reuse. That said, I still like using AsyncTask for discrete tasks in an Android application, since it provides a nice abstraction
for cleanly defining that task. Did I leave out an approach that you like? Leave a comment and let me know!
*/