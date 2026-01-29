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

**Note: ensure to add the proper `using` or directive statements in the ARS.cs/COPY.cs file**


<details>
  <summary><b>❓ Cool, but what do I need to do?</b></summary>

Not much, really.  
This project uses the **MIT License** ([LICENSE](LICENSE)).

That means you can:
- use it
- change it
- ship it
- sell it
- probably do something I never intended

Just a couple things:
- keep the copyright + license notice
- don’t say I wrote your whole project
- don’t blame me if it breaks

That’s literally it.

</details>
