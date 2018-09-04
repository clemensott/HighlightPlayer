VideoUltimate v1.5.4.0
.NET Video Reader and Thumbnailer
Copyright © 2016-2017 GleamTech
https://www.gleamtech.com/videoultimate

Version 1.5.4 Release Notes:
https://docs.gleamtech.com/videoultimate/html/version-history.htm#v1.5.4

Online Documentation:
https://docs.gleamtech.com/videoultimate/

Support Portal:
https://support.gleamtech.com/

------------------------------------------------------------------------------------------------------
Information on package contents:
------------------------------------------------------------------------------------------------------

  - "Bin" folder contains the DLLs for VideoUltimate. The DLLs are targetted for .NET 4.0 and above.
    Please see below for the full instructions on how to reference and use the DLLs in your .NET project.

  - "Examples" folder contains the example projects for ASP.NET Web Forms (C# and VB) and ASP.NET MVC (C# and VB). 
    Please open one of the solution files with Visual Studio 2017/2015/2013/2012/2010 to load an example project. 
    The example projects demonstrate various features and settings of VideoUltimate classes. Note that, 
    the projects reference GleamTech.VideoUltimate.dll which is at "Bin" folder of the root of this package 
    so make sure you extract the whole package (not only "Examples" folder) before opening a solution.

  - "Help" folder contains the API reference as a CHM file.

------------------------------------------------------------------------------------------------------
To use VideoUltimate in a project, do the following in Visual Studio:
------------------------------------------------------------------------------------------------------

1.  Set VideoUltimate's global configuration. For example, you may want to set the license key 
    Insert the following line into the Application_Start method of your Global.asax.cs for Web projects 
    or Main method for other project types:

    ----------------------------------
    //Set this property only if you have a valid license key, otherwise do not
    //set it so VideoUltimate runs in trial mode.
    VideoUltimateConfiguration.Current.LicenseKey = "QQJDJLJP34...";
    ----------------------------------

    Alternatively you can specify the configuration in <appSettings> tag of your Web.config (or App.exe.config).

    ----------------------------------
    <appSettings>
      <add key="VideoUltimate:LicenseKey" value="QQJDJLJP34..." />
    </appSettings>
    ----------------------------------

    As you would notice, VideoUltimate: prefix maps to VideoUltimateConfiguration.Current.

2.  Open one of your class files (eg. Program.cs) and at the top of your file add GleamTech.VideoUltimate namespace:

    ----------------------------------
    using GleamTech.VideoUltimate;
    ----------------------------------

    Now in some method insert these lines:

    ----------------------------------
    using (var videoFrameReader = new VideoFrameReader(@"C:\Video.mp4"))
    {
        if (videoFrameReader.Read())
        {
            using (var frame = videoFrameReader.GetFrame())
                frame.Save(@"C:\Frame1.jpg", ImageFormat.Jpeg);
        }
    }
    ----------------------------------

    This will open the source video C:\Video.mp4, read the first frame, and if the frame is read and 
    decoded successfully, it will get a Bitmap instance of the frame and save it as C:\Frame1.jpg.

    Sometimes you may only need to quickly generate a meaningful thumbnail for a video, you can use 
    VideoThumbnailer class for this:
    
    ----------------------------------
    using (var videoThumbnailer = new VideoThumbnailer(@"C:\Video.mp4"))
    using (var thumbnail = videoThumbnailer.GenerateThumbnail(100))
        thumbnail.Save(@"C:\Thumbnail1.jpg", ImageFormat.Jpeg);	
    ----------------------------------	