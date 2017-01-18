// App refrence. "http://gregshackles.com/using-background-threads-in-mono-for-android-applications/"
using Android.App;
using Android.OS;
using System.Threading.Tasks;
using System.Threading;

namespace TinyThreading
{
	[Activity(Label = "ThreadingDemo", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		private LoginService _loginService;
		private ProgressDialog _progressDialog;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			_loginService = new LoginService();
			_progressDialog = new ProgressDialog(this) { Indeterminate = true };
			_progressDialog.SetTitle("Login In Progress");
			_progressDialog.SetMessage("Please wait...");

			loginWithTaskLibrary();
		}

		// this method will run the threaded login code in Main thread due to which it blocks execution of _progressDialog and is not shown.
		private void loginSynchronously()
		{
			_progressDialog.Show();

			_loginService.Login("greg");

			onSuccessfulLogin();
		}

		// code with .net thread object.
		// this method will run the threaded login code in another thread and update view objects from ui thread, so _progressdialg execution is not halted
		// and this is the preferred way of handling threads to test, change method call in line 22 to this and above synchronous method.
		private void loginWithThread()
		{
			_progressDialog.Show();

			new Thread(new ThreadStart(() =>
			{
				_loginService.Login("greg");

				RunOnUiThread(() => onSuccessfulLogin());
			})).Start();
		}

		// code with java thread object.
		private void loginWithJavaThread()
		{
			_progressDialog.Show();

			new Java.Lang.Thread(() =>
			{
				_loginService.Login("greg");

				RunOnUiThread(() => onSuccessfulLogin());
			}).Start();
		}

		// login with thread pool. this is the much cleaner approach for working with threads.
		private void loginWithThreadPool()
		{
			_progressDialog.Show();

			ThreadPool.QueueUserWorkItem(state =>
			{
				_loginService.Login("greg");

				RunOnUiThread(() => onSuccessfulLogin());
			});
		}

		// this method will run the login service through AsyncClass, in Task folder. The Android way or cleaner approach. The callbacks on** will
		// run in UI thread implicitly.
		private void loginWithAsyncTask()
		{
			new LoginTask(this, _loginService).Execute("Milan");
		}

		// this method will use Task parellel library for backgrounding and is the mono way of handling backgrounding, this is the cleanest approach,
		//dig deeper on this. notice that we have used using System.Threading.Tasks; using System.Threading; try removing them and look what IDE does.
		private void loginWithTaskLibrary()
		{
			_progressDialog.Show();

			Task.Factory
				.StartNew(() =>
					_loginService.Login("greg")
				)
				.ContinueWith(task =>
					RunOnUiThread(() =>
						onSuccessfulLogin()
					)
				);
		}

		private void onSuccessfulLogin()
		{
			_progressDialog.Hide();

			new AlertDialog.Builder(this)
				.SetTitle("Login Successful")
				.SetMessage("Great success!")
				.Show();
		}
	}
}
