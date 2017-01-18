using System.Threading;

namespace TinyThreading
{
	public class LoginService
	{
		public void Login(string username)
		{
			Thread.Sleep(10000);
		}
	}
}