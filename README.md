# ARS 
## Automatic Reporting System
This is a mod that automatically reports users on the ARSPlayerIDs repository. This is made to be added into other mods, and to do that, all you need to do is:

1. Copy the code in [COPY.cs](COPY.cs) and add it to your project
2. In your main Start or Awake method, add the following:
```C#
GameObject arsobj = new GameObject("ARS");
DontDestroyOnLoad(arsobj);
arsobj.AddComponent<ARS.ARSComponent>();
```
3. You are done! ARS is now fully implemented into your project.

**Note: insure to add the proper `using` or directive statements in the ARS.cs/COPY.cs file**
