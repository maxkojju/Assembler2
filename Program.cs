using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using SkiaSharp;
using OpenCvSharp;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;
using SkiaSharp.Views.Desktop;
using System.Numerics;
using System.Xml.Linq;

namespace Assembler2
{
	internal class Program
	{
		static public bool debug = false;
		static DisplayForm displayForm = new DisplayForm(1,1);


		static string CodePath = "";
		static string SourceCode = @"";


		#region code goes here
		static Dictionary<string, string> stringHolders = new Dictionary<string, string>();
		static Dictionary<string, float> floatHolders = new Dictionary<string, float>();
		static List<Vector4> screenBuffer = new List<Vector4>();
		//только стринги и флоты!

		static void Main(string[] args)
		{
			Console.WriteLine("input path to .ass2 file");
		if(CodePath == "")
			SourceCode = File.ReadAllText(Console.ReadLine());
			else SourceCode = File.ReadAllText(CodePath);

			int line = 0;
			var surface = SKSurface.Create(new SKImageInfo(400, 300));
			SKCanvas canvas = surface.Canvas;
			canvas.Clear(SKColors.White);

			if (debug) if (debug) Console.WriteLine("OUTPUT:			DEV LOGS:");
			while (line < SourceCode.Split(';').Length)
			{
			string code = SourceCode.Split(';')[line];
			code = code.Trim();
			
			line++;
				if (code.StartsWith("//") || code.StartsWith("#"))
				{
					if (debug) Console.WriteLine("			"+code);
				}if (code.StartsWith("@"))
				{
					if (!code.StartsWith("@!"))
					{
						int newLn = findStr(SourceCode, "@!" + code.Split('@')[1]
							.Split(')')[0]);
						line = newLn;
					}
				}
				if(code.StartsWith("include<window>"))
				{
					float Value2 = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value = getFlt(code.Split(',')[1].Split(')')[0]);
					LaunchWindowAsync(Value2,Value);
				}
				else

					if (code.StartsWith("newstr("))
				{
					string Name = getStr(code.Split('(')[1].Split(')')[0]);
					newStr(Name);
					if(debug) Console.WriteLine("\t\t\tnewstr : " + Name);
				}
				else
				if(code.StartsWith("setstr("))
				{
					string Name = getStr(code.Split('(')[1].Split(',')[0]);
					string Value = getStr(code.Split(',')[1].Split(')')[0]);

					setStr(Name, Value);
					if (debug) Console.WriteLine("			edit : " + Name + " @ " + Value);
				}else
				if (code.StartsWith("setflt("))
				{
					string Name = getStr(code.Split('(')[1].Split(',')[0]);
					float Value = getFlt(code.Split(',')[1].Split(')')[0]);

					setFlt(Name, Value);
					if (debug) Console.WriteLine("			edit float : " + Name + " @ " + Value);
				}else
				if (code.StartsWith("newflt("))
				{
					string Name = getStr(code.Split('(')[1].Split(')')[0]);

					newFlt(Name);
					if (debug) Console.WriteLine("			new Flt :" + Name);
				}else
				if (code.StartsWith("print("))
				{
					string Name = getStr(code.Split('(')[1].Split(')')[0]);
					Console.WriteLine(Name);
				}else
				if (code.StartsWith("printflt("))
				{
					float Name;
					string source = code.Split('(')[1].Split(')')[0];
					if (source.StartsWith("!")) {
						string PreName = getStr(code.Split('(')[1].Split(')')[0]);
						 Name = getFlt("!" +PreName); 
					}else
					{
						 Name = getFlt("!"+source);
					}

					Console.WriteLine(Name);
				}else
				if(code.StartsWith("add")){
					float Value = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					string Name = getStr(code.Split(',')[2].Split(')')[0]);
					if (debug) Console.WriteLine("			add :" + Value + "+"+Value2+ " into-" +Name);
					setFlt(Name, Value+Value2);
				}
				else
				if (code.StartsWith("sub"))
				{
					float Value = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					string Name = getStr(code.Split(',')[2].Split(')')[0]);
					if (debug) Console.WriteLine("			substract :" + Value + "+" + Value2);
					setFlt(Name, Value - Value2);
				}
				else
				if (code.StartsWith("mult"))
				{
					float Value = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					string Name = getStr(code.Split(',')[2].Split(')')[0]);
					if (debug) Console.WriteLine("			multiply :" + Value + "+" + Value2);
					setFlt(Name, Value * Value2);
				}
				else
				if (code.StartsWith("div"))
				{
					float Value = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					string Name = getStr(code.Split(',')[2].Split(')')[0]);
					if (debug) Console.WriteLine("			divide :" + Value + "+" + Value2);
					setFlt(Name, Value / Value2);
				}else if(code.StartsWith("reflt"))
				{
					
					string Value = getStr(code.Split('(')[1].Split(',')[0]);
					float Val = getFlt("!"+Value);
					string Value2 = getStr(code.Split(',')[1].Split(')')[0]);
					if (debug) Console.WriteLine("			reflt :" +Value+" in "+ Val + " of " + Value2);
					setFlt(Value2, Val);
				}
				else
				if (code.StartsWith("combine"))
				{
					string Value = getStr(code.Split('(')[1].Split(',')[0]);
					string Value2 = getStr(code.Split(',')[1]);
					string Name = getStr(code.Split(',')[2].Split(')')[0]);
					if (debug) Console.WriteLine("			merge :" + Value + "+" + Value2);
					setStr(Name, Value + Value2);
				}else
				if (code.StartsWith("random"))
				{
					float Value = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					string Name = getStr(code.Split(',')[2].Split(')')[0]);
					if (debug) Console.WriteLine("			merge :" + Value + "+" + Value2);
					setFlt(Name, new Random().Next((int)Value,(int)Value2));
				}else if (code.StartsWith("str<flt"))
				{
					string Name = getStr(code.Split('(')[1].Split(',')[0]);
					float Value = getFlt(code.Split(',')[1].Split(')')[0]);
					setStr(Name,Value.ToString());
				}
				else
				if (code.StartsWith("goto"))
				{
					string Name = getStr(code.Split('(')[1].Split(')')[0]);
					int newLn = findStr(SourceCode, Name);
					if (debug) Console.WriteLine("			goto :" +newLn +"}{"+Name);
					line = newLn;
				}
				if (code.StartsWith("if>"))
				{
					float Value1 = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					string Name = getStr(code.Split(',')[2].Split(')')[0]);
					if (debug) Console.WriteLine($"			check {Value1} > {Value2};");
					if (Value1 <= Value2)
					{
						int newLn = findStr(SourceCode, Name);
						if (debug) Console.WriteLine("			goto :" + newLn + "}{" + Name);
						line = newLn;
					}

				}
				else
				if (code.StartsWith("if<"))
				{
					float Value1 = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					string Name = getStr(code.Split(',')[2].Split(')')[0]);
					if (debug) Console.WriteLine($"			check {Value1} < {Value2};");
					if (Value1 >= Value2)
					{
						int newLn = findStr(SourceCode, Name);
						if (debug) Console.WriteLine("			goto :" + newLn + "}{" + Name);
						line = newLn;
					}

				}
				else
				if (code.StartsWith("if="))
				{
					float Value1 = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					string Name = getStr(code.Split(',')[2].Split(')')[0]);
					if (debug) Console.WriteLine($"			check {Value1} == {Value2};");
					if (Value1 == Value2)
					{
						int newLn = findStr(SourceCode, Name);
						if (debug) Console.WriteLine("			goto :" + newLn + "}{" + Name);
						line = newLn;
					}

				}
				else
				if (code.StartsWith("if!="))
				{
					float Value1 = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					string Name = getStr(code.Split(',')[2].Split(')')[0]);
					if (debug) Console.WriteLine($"			check {Value1} != {Value2};");
					if (Value1 != Value2)
					{
						int newLn = findStr(SourceCode, Name);
						if (debug) Console.WriteLine("			goto :" + newLn + "}{" + Name);
						line = newLn;
					}

				}
				else if (code.StartsWith("setsize"))
				{
					float Value1 = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1].Split(')')[0]);

					using (var paint = new SKPaint { Color = SKColors.Red })
					{
						screenBuffer.Clear();
						surface = SKSurface.Create(new SKImageInfo((int)Value1, (int)Value2));
						canvas = surface.Canvas;
					}
				}
				else if (code.StartsWith("paint"))
				{
					float Value1 = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					float Value3 = getFlt(code.Split(',')[2]);
					float Value4 = getFlt(code.Split(',')[3].Split(')')[0]);
					screenBuffer.Add(new Vector4(Value1, Value2, Value3, Value4));

					float Color1 = getFlt(code.Split('/')[1]);
					float Color2 = getFlt(code.Split('/')[2]);
					float Color3 = getFlt(code.Split('/')[3].Replace(";", ""));
					SKColor color = new SKColor((byte)Color1, (byte)Color2, (byte)Color3);
					using (var paint = new SKPaint { Color = color })
					{
						canvas.DrawRect(Value1, Value2, Value3, Value4, paint);
					}
				}
				else if (code.StartsWith("line"))
				{
					float Value1 = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					float Value3 = getFlt(code.Split(',')[2]);
					float Value4 = getFlt(code.Split(',')[3].Split(')')[0]);

					float Color1 = getFlt(code.Split('/')[1]);
					float Color2 = getFlt(code.Split('/')[2]);
					float Color3 = getFlt(code.Split('/')[3].Replace(";", ""));
					SKColor color = new SKColor((byte)Color1, (byte)Color2, (byte)Color3);
					using (var paint = new SKPaint { Color = color })
					{
						canvas.DrawLine(Value1, Value2, Value3, Value4, paint);
					}
				}
				else if (code.StartsWith("bgpaint"))
				{
					float Value1 = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					float Value3 = getFlt(code.Split(',')[2]);
					float Value4 = getFlt(code.Split(',')[3].Split(')')[0]);

					float Color1 = getFlt(code.Split('/')[1]);
					float Color2 = getFlt(code.Split('/')[2]);
					float Color3 = getFlt(code.Split('/')[3].Replace(";", ""));
					SKColor color = new SKColor((byte)Color1, (byte)Color2, (byte)Color3);
					using (var paint = new SKPaint { Color = color })
					{
						canvas.DrawRect(Value1, Value2, Value3, Value4, paint);
					}
				}
				else if (code.StartsWith("show"))
				{
					float Value1 = getFlt(code.Split('(')[1].Split(')')[0]);

					displayForm.Redraw(surface);
					System.Threading.Thread.Sleep((int)Math.Round(Value1 * 10));
				}
				else if (code.StartsWith("if key"))
				{
					string Value1 = getStr(code.Split('(')[1].Split(',')[0]);
					string Value2 = getStr(code.Split(',')[1].Split(')')[0]);

					if (TryParseConsoleKey(Value1, out ConsoleKey key))
						if (!IsKeyPressed(key))
						{
							int newLn = findStr(SourceCode, Value2);
							if (debug) Console.WriteLine("			goto :" + newLn + "}{" + Value2);
							line = newLn;
						}
				}
				else if (code.StartsWith("key"))
				{
					string Value1 = getStr(code.Split('(')[1].Split(')')[0]);

					ConsoleKeyInfo keyInfo = Console.ReadKey();
					string a = "";
					a = keyInfo.KeyChar.ToString();
					if (keyInfo.Key == ConsoleKey.Backspace)
						a = "BackSpace";
					if (keyInfo.Key == ConsoleKey.Enter)
						a = "\n";

					setStr(Value1, a.ToString());

				}
				else if (code.StartsWith("if[str]="))
				{
					string Value1 = getStr(code.Split('(')[1].Split(',')[0]);
					string Value2 = getStr(code.Split(',')[1]);
					string Name = getStr(code.Split(',')[2].Split(')')[0]);
					if (debug) Console.WriteLine($"			checkstr {Value1} == {Value2};");
					if (Value1 != Value2)
					{
						int newLn = findStr(SourceCode, Name);
						if (debug) Console.WriteLine("			goto :" + newLn + "}{" + Name);
						line = newLn;
					}
				}
				else if (code.StartsWith("input"))
				{
					string inp = Console.ReadLine();
					string Name = getStr(code.Split('(')[1].Split(')')[0]);
					setStr(Name, inp);
				}
				else if (code.StartsWith("flt<str"))
				{
					string Name = getStr(code.Split('(')[1].Split(',')[0]);
					string Value = getStr(code.Split(',')[1].Split(')')[0]);
					try
					{
						setFlt(Name, float.Parse(Value));
					}
					catch
					{
						setFlt(Name, -1);
					}

				}
				else if (code.StartsWith("overlap?"))
				{
					float Value1 = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					float Value3 = getFlt(code.Split(',')[2]);
					float Value4 = getFlt(code.Split(',')[3]);
					string Name = getStr(code.Split(',')[4]).Split(')')[0];

					if (IntersectsAny(new Vector4(Value1, Value2, Value3, Value4), screenBuffer))
					{
						int newLn = findStr(SourceCode, Name);
						if (debug) Console.WriteLine("			goto(overlap) :" + newLn + "}{" + Name);
						line = newLn;
					}
				}
				else if (code.StartsWith("!overlap?"))
				{
					float Value1 = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					float Value3 = getFlt(code.Split(',')[2]);
					float Value4 = getFlt(code.Split(',')[3]);
					string Name = getStr(code.Split(',')[4]).Split(')')[0];

					if (!IntersectsAny(new Vector4(Value1, Value2, Value3, Value4), screenBuffer))
					{
						int newLn = findStr(SourceCode, Name);
						if (debug) Console.WriteLine("			goto(!overlap) :" + newLn + "}{" + Name);
						line = newLn;
					}
				}
				else if (code.StartsWith("text"))
				{
					float Value1 = getFlt(code.Split('(')[1].Split(',')[0]);
					float Value2 = getFlt(code.Split(',')[1]);
					float Value3 = getFlt(code.Split(',')[2]);
					string Value4 = getStr(code.Split(',')[3].Split(')')[0]);

					float Color1 = getFlt(code.Split('/')[1]);
					float Color2 = getFlt(code.Split('/')[2]);
					float Color3 = getFlt(code.Split('/')[3].Replace(";", ""));

					SKColor color = new SKColor((byte)Color1, (byte)Color2, (byte)Color3);
					using (var paint = new SKPaint { Color = color, TextSize = Value3 })
					{
						canvas.DrawText(Value4, Value1, Value2, paint);
					}

					Cv2.WaitKey(0);
					Cv2.DestroyAllWindows();
				}
				else if (code.StartsWith("@!"))
				{
					if (LastLines.Count != 0)
					{
						line = LastLines[LastLines.Count - 1];
						LastLines.RemoveAt(LastLines.Count - 1);
					}
				}
				else if(code.StartsWith("("))
				{
					LastLines.Add(line);
					int newLn = findStr(SourceCode, "@" + code.Split('(')[1]
					.Split(')')[0])+1;
					line = newLn;
				}
				
			}
		}
		#endregion
		#region funcs

		static List<int> LastLines = new List<int>();
		public static bool TryParseConsoleKey(string input, out ConsoleKey consoleKey)
		{
			consoleKey = default;

			if (string.IsNullOrWhiteSpace(input))
				return false;
			
				if (Enum.TryParse<ConsoleKey>(input, true, out consoleKey))
			{
				return true;
			}

			if (input.Length == 1)
			{
				char ch = char.ToUpper(input[0]);
				if (char.IsLetterOrDigit(ch))
				{
					string keyName = ch.ToString();
					if (Enum.TryParse<ConsoleKey>(keyName, true, out consoleKey))
					{
						return true;
					}
				}
			}

			switch (input.ToLower())
			{
				case "enter":
					consoleKey = ConsoleKey.Enter;
					return true;
				case "space":
				case "spacebar":
					consoleKey = ConsoleKey.Spacebar;
					return true;
				case "esc":
				case "escape":
					consoleKey = ConsoleKey.Escape;
					return true;
				case "tab":
					consoleKey = ConsoleKey.Tab;
					return true;
				default:
					return false;
			}
		}
		private static IntPtr SetHook(LowLevelKeyboardProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
					GetModuleHandle(curModule.ModuleName), 0);
			}
		}
		private const int WH_KEYBOARD_LL = 13; 
		private const int WM_KEYDOWN = 0x0100; 
		private static LowLevelKeyboardProc _proc = HookCallback;
		private static IntPtr _hookID = IntPtr.Zero;
		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
			{
				int vkCode = Marshal.ReadInt32(lParam);
				Console.WriteLine((Keys)vkCode); 
			}
			return CallNextHookEx(_hookID, nCode, wParam, lParam);
		}
		public static bool IntersectsAny(Vector4 target, List<Vector4> screenBuffer)
		{
			foreach (Vector4 square in screenBuffer)
			{
				if (Intersects(target, square))
				{
					return true; 
				}
			}
			return false; 
		}

		private static bool Intersects(Vector4 a, Vector4 b)
		{
			float x1 = a.X;
			float y1 = a.Y;
			float w1 = a.Z;
			float h1 = a.W;

			float x2 = b.X;
			float y2 = b.Y;
			float w2 = b.Z;
			float h2 = b.W;

			// Проверка пересечения по осям X и Y
			return (x1 + w1 > x2 && x2 + w2 > x1 && y1 + h1 > y2 && y2 + h2 > y1);
		}
		public static async void key()
		{
			_hookID = SetHook(_proc);
			await Task.Run(() => Application.Run());

			}
		// Импорт функций Windows API
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn,
			IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
			IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);
		[DllImport("user32.dll")]
		private static extern short GetAsyncKeyState(int vKey);

		public static bool IsKeyPressed(ConsoleKey key)
		{

			int vKey = (int)key;
			return (GetAsyncKeyState(vKey) & 0x8000) != 0;
		}

		static int findStr(string source,string target)
		{
			string[] strings = source.Split(';');
			for (int i = 0; i < strings.Length; i++)
			{
			if(target.Trim().StartsWith("@"))
			{
				if (strings[i].Trim() == target.Trim()) return i;
			}
			else
				if (strings[i].Trim() == "#" + target.Trim()) return i;
			}
			return -1;
		}
		static void setStr(string Name, string Val)
		{
			stringHolders[Name.Trim()] = Val;
		}
		static void newStr(string Name)
		{
		stringHolders.Add(Name.Trim(), "undefined");
		}
		static void setFlt(string Name, float Val)
		{
			if (!floatHolders.ContainsKey(Name.Trim()))
				floatHolders.Add(Name.Trim(), -1);

			floatHolders[Name.Trim()] = Val;
		}
		static void newFlt(string Name)
		{
			if (!floatHolders.ContainsKey(Name)) floatHolders.Add(Name.Trim(), -1); else floatHolders[Name] = 0;
		}

		private static void LaunchWindowAsync(float val1,float val2)
		{
			var thread = new Thread(() =>
			{
				displayForm = new DisplayForm(val1,val2);
				
				Application.Run(displayForm);
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}
		static float getFlt(string Val)
		{
		
			if (debug) Console.WriteLine("			reading float " + Val);

			if (Val.Trim().StartsWith("!"))
			{ if (floatHolders.ContainsKey(Val.Trim().Substring(1)))
					return floatHolders[Val.Trim().Substring(1)];
				else return -2;
			}
			else
			{
				if (debug) Console.WriteLine("			parsing " + (Val.Trim()));
				return float.Parse(Val.Trim().Replace('.',','));
			}

		}
		static string getStr(string Val)
		{
			if (debug) Console.WriteLine("			reading " + Val);


			if (Val.Trim().StartsWith("!"))
				return stringHolders[Val.Trim().Substring(1)];
			else
				return Val;
		
		}

		public class DisplayForm : Form
		{
			private SKControl skControl = new SKControl();
			private SKImage currentImage; 

			public DisplayForm(float x,float y)
			{
				skControl.Dock = DockStyle.Fill;
				Controls.Add(skControl);
				Size = new System.Drawing.Size((int)x, (int)y); 
				Text = "Assembler2 cool Window"; 
				skControl.PaintSurface += SkControl_PaintSurface; 
			}

			private void SkControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
			{
				var canvas = e.Surface.Canvas;
				canvas.Clear(); 

				if (currentImage != null)
				{
					canvas.DrawImage(currentImage, 0, 0);
				}
			}

			public void Redraw(SKSurface surface)
			{
				if (InvokeRequired)
				{
					Invoke(new Action<SKSurface>(Redraw), surface);
					return;
				}

				using (var snapshot = surface.Snapshot())
				{
					currentImage?.Dispose();
					currentImage = SKImage.FromBitmap(SKBitmap.FromImage(snapshot));
				}

				skControl.Invalidate(); 
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					currentImage?.Dispose();
				}
				base.Dispose(disposing);
			}
		}
	}
}

#endregion