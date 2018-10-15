/*
 * Created by SharpDevelop.
 * User: Левченко
 * Date: 04.10.2018
 * Time: 11:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Windows.Media.Imaging;

namespace WPF
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			//Timer timer = new Timer { Interval = 1000 };
			InitializeComponent();
			
			WindowPole win_pole = new WindowPole(this) {Width = 515, Height = 535};
			win_pole.Show();
			Close();

		}
	}
	
	public class WindowPole : Window
	{
		public WindowPole(Window main_win)
		{
			//InitializeComponent();
			
			int width_array = 10;
			int height_array = 10;
			int count_bomb = 10;
			int width_btn = 50;
			int height_btn = 50;
			
			Grid grid1 = new Grid();
			this.AddChild(grid1);
			
			Button[,] button = new Button[height_array, width_array];
			Pole pole = new Pole(height_array, width_array, count_bomb);
			
			pole.NewPole();
			
			for (int i = 0; i < button.GetLength(0); i++)
				for (int j = 0; j < button.GetLength(1); j++)
				{
				button[i,j] = new Button() { Width = width_btn, Height = height_btn,
					HorizontalAlignment = HorizontalAlignment.Left, 
					VerticalAlignment = VerticalAlignment.Top, 
					Margin = new Thickness(j*width_btn, i*height_btn, 0, 0),
					Tag = 0
					//Content = pole.pole[i,j].ToString()
					};
				button[i,j].Click += delegate(object sender, RoutedEventArgs e){pole.OnClick(sender, e, button);};
				button[i,j].MouseRightButtonUp += delegate(object sender, MouseButtonEventArgs e){pole.OnClickRight(sender, e, button);};
				grid1.Children.Add(button[i,j]);
				}
		}
		
//		public void Loose()
//		{
//			MessageBoxResult res = MessageBox.Show("Все по честному", "Ты проиграл", MessageBoxButton.YesNo);
//				switch (res) 
//				{
//					case (MessageBoxResult.Yes):
//						WindowPole win_pole = new WindowPole(main_win);
//						win_pole.Show();
//						Close();
//						break;
//						
//					case(MessageBoxResult.No):
//						Close();
//						break;
//				}
//		}
	}
	
	
	public class Pole
	{
		private int[,] pole;
		private int CountBomb {set; get;} //количество бомб
		private int CountRightClick {set; get;} //количество помеченных бомб
		private int CountButton {set; get;} //количество не открытых клеток
		Random rnd = new Random();
		List<Image> img = new List<Image>(10);
		Window win;
		
		
		public Pole(int i, int j, int count)
		{
			pole = new int[i, j];
			CountBomb = count;
			CountRightClick = 0;
			CountButton = i*j;
			this.win = win;
		}
		
		public void NewPole()
		{
			
			for (int i = 0; i < CountBomb; i++)
			{
				img.Add(new Image());
				img[i].Source = new BitmapImage(new Uri(@"D:\Prjct\WPF\WPF\bin\Debug\bomb.png"));
			}
			
			for (int i = 0; i < pole.GetLength(0); i++)
				for (int j = 0; j < pole.GetLength(1); j++)
					pole[i,j] = 0;
			
			int iTemp, jTemp, countBombTemp;
			countBombTemp = CountBomb;
			while (countBombTemp > 0)
			{
				iTemp = rnd.Next(0, pole.GetLength(0));
				jTemp = rnd.Next(0, pole.GetLength(1));
				if (pole[iTemp, jTemp] == 0)
				{
					pole[iTemp, jTemp] = -1;
					countBombTemp--;
				}
			}
			
			for (int i = 0; i < pole.GetLength(0); i++)
				for (int j = 0; j < pole.GetLength(1); j++)
					if (pole[i,j] == -1)
						AroundBombNumber(i, j);
		}
		
		private void AroundBombNumber(int i, int j)
		{
			if (i > 0)
			{
				if (j > 0)
					if (pole[i-1, j-1] != -1) pole[i-1, j-1]++;
				if (pole[i-1, j] != -1) pole[i-1, j]++;
				if (j < pole.GetLength(1)-1)
					if(pole[i-1, j+1] != -1) pole[i-1, j+1]++;
			}
			
			if (j > 0)
				if(pole[i, j-1] != -1) pole[i, j-1]++;
			if (j < pole.GetLength(1)-1)
				if(pole[i, j+1] != -1) pole[i, j+1]++;
			
			if (i < pole.GetLength(0)-1)
			{
				if (j > 0)
					if(pole[i+1, j-1] != -1) pole[i+1, j-1]++;
				if(pole[i+1, j] != -1) pole[i+1, j]++;
				if (j < pole.GetLength(1)-1)
					if(pole[i+1, j+1] != -1) pole[i+1, j+1]++;
			}
		}
		
		private void AroundNullNumber(int i, int j, Button[,] button)
		{
			if (pole[i,j] > 0)
			{
				button[i,j].IsEnabled = false;
				button[i,j].Content = pole[i,j].ToString();
				pole[i,j] = -2;
				CountButton--;
			}
			else
				if (pole[i,j] == 0)
				{
					button[i,j].Visibility = Visibility.Hidden;
					pole[i,j] = -2;
					CountButton--;
					if (i > 0)
					{
						if (j > 0)
							AroundNullNumber(i-1, j-1, button);
					
						AroundNullNumber(i-1, j, button);
						
						if (j < pole.GetLength(1)-1)
							AroundNullNumber(i-1, j+1, button);
					}
					
					if (j > 0)
						AroundNullNumber(i, j-1, button);
					
					
					if (j < pole.GetLength(1)-1)
						AroundNullNumber(i, j+1, button);
					
					
					if (i < pole.GetLength(0)-1)
					{
						if (j > 0)
							AroundNullNumber(i+1, j-1, button);
					
						AroundNullNumber(i+1, j, button);
						
						if (j < pole.GetLength(1)-1)
							AroundNullNumber(i+1, j+1, button);
					}
				}
		}

		public void OnClick(object sender, RoutedEventArgs e, Button[,] button)
		{
			int i = 0; 
			int j = 0;
			bool flag = false;
			for (i = 0; i < button.GetLength(0); i++)
			{
				for (j = 0; j < button.GetLength(1); j++)
					if (button[i,j] == (Button)sender)
					{
						flag = true;
						break;
					}
				if (flag)
					break;
			}
			if (pole[i,j] == -1)
			{
				int z = 0;
				for (i = 0; i < pole.GetLength(0); i++)
				{
					for (j = 0; j < pole.GetLength(1); j++)
					{
						if (pole[i,j] == -1)
						{ 
							button[i,j].Content = img[z];
							z++;
						}
						else button[i,j].IsEnabled = false;
					}
				}
				
				MessageBox.Show("Все по честному");
			}
			else AroundNullNumber(i,j,button);
			
			
			
			if (CountButton == CountBomb || CountRightClick == CountBomb)
				MessageBox.Show("Это вин");
		}
		
		public void OnClickRight(object sender, RoutedEventArgs e, Button[,] button)
		{
			int i = 0; 
			int j = 0;
			bool flag = false;
			for (i = 0; i < button.GetLength(0); i++)
			{
				for (j = 0; j < button.GetLength(1); j++)
					if (button[i,j] == (Button)sender)
					{
						flag = true;
						break;
					}
				if (flag)
					break;
			}
			if (int.Parse(button[i,j].Tag.ToString()) == 0)
			{
				button[i,j].Tag = 1;
				button[i,j].Content = "?";
				if (pole[i,j] == -1) CountRightClick++;
			}
			else
			{
				button[i,j].Tag = 0;
				button[i,j].Content = "";
				if (pole[i,j] == -1) CountRightClick--;
			}
			
			if (CountRightClick == CountBomb)
				MessageBox.Show("Это вин");
			
		}
		
		
	}
}