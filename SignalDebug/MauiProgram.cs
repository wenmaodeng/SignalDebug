﻿using SignalDebug.Services;

namespace SignalDebug;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
        builder.Services.AddSingleton<Views.MainPage>();
        builder.Services.AddSingleton<DataSignalDatabase>();
        builder.Services.AddSingleton<BluetoothLeService>();
        return builder.Build();
	}
}
