using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SiberianWellness.Common.ShellWrapper
{
	public class ShellWrapper
	{
		string baseString;
		string WorkingDirectory;

		public bool printInput =true;
		public bool printResult =true;
		public bool printError =true;
		
		public string Error  { get; private set; }
		public string Result { get; private set; }

		public bool HaveError => !string.IsNullOrEmpty(Error);

		public ShellWrapper(string baseString, string WorkingDirectory)
		{
			this.baseString       = baseString;
			this.WorkingDirectory = WorkingDirectory;
			
		}

		public bool SuccessLastOrPrintError()
		{
			if (HaveError)
			{
				Debug.LogError(Error);
				return false;
			}
			else
			{
				return true;
			}
		}

		public void Run(string args)
		{
			Process cmd = new Process();
			cmd.StartInfo.FileName               = baseString;
			cmd.StartInfo.WorkingDirectory       = WorkingDirectory;
			cmd.StartInfo.RedirectStandardInput  = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.StartInfo.RedirectStandardError  = true;
			cmd.StartInfo.Arguments  = args; 
			
			cmd.StartInfo.CreateNoWindow  = false;
			cmd.StartInfo.UseShellExecute = false;
			
			//Кодровки для нормального отображения русского языка в консоли юнити
			cmd.StartInfo.StandardErrorEncoding  = Encoding.GetEncoding("cp866");
			cmd.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("cp866");

			if(printInput)
				Debug.Log(baseString + " "+ args);
		 
			try
			{
				cmd.Start(); 
				//cmd.StandardInput.WriteLine(args);  
				//cmd.StandardInput.Flush();
				//cmd.StandardInput.Close();
				cmd.WaitForExit(10000);

				Result = cmd.StandardOutput.ReadToEnd();
				Error  = cmd.StandardError.ReadToEnd();
			}
			catch (Exception e)
			{
				Debug.LogError($"ShellWrapper ошибка при запуске команды '{baseString} {args}' рабочая папка '{WorkingDirectory}'");
				Debug.LogError(e);
				throw e;
			}
			
			if(printError && HaveError)
				Debug.LogError(Error);
			
			if(printResult && !string.IsNullOrEmpty(Result))
				Debug.Log(Result);
		
			
			// List<byte> read= new List<byte>(1024);
			// int r = 0;
			// while (r != -1)
			// {
			// 	r = cmd.StandardError.BaseStream.ReadByte();
			// 	if (r > -1 && r < 256)
			// 		read.Add((byte) r);
			// }
			//
			// var bytes = read.ToArray();
			//
			// string msg = "";
			//
			// foreach (var c in Encoding.GetEncodings())
			// {
			// 	var e = c.GetEncoding();
			// 	char[] asciiChars = new char[e.GetCharCount(bytes, 0, bytes.Length)];
			// 	e.GetChars(bytes, 0, bytes.Length, asciiChars, 0);
			// 	string s = new string(asciiChars);
			//
			// 	msg += s; 
			// 	msg += "     " +c.Name + " " + c.DisplayName + Environment.NewLine;
			// }
			//
			// Debug.Log(msg);
			//
			// Encoding ascii   = Encoding.ASCII;
			// Encoding unicode = Encoding.Unicode;
			//
			// // Perform the conversion from one encoding to the other.
			// byte[] asciiBytes = Encoding.Convert(unicode, ascii, bytes); 
		}
	}
}