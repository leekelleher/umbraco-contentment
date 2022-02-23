<img src="../../docs/assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Demo Umbraco website

This is a demo website, using Umbraco v8.17.0, to showcase the features of the Contentment package.

It should be considered as a continual work-in-progress.

It could have the potential to be serve as a starter kit, however it is opinionated, in that it used uSync and ModelsBuilder configurations.

Umbraco vversion `8.17.0` is intentionally used, as that is the minimum version of Umbraco that latest version of Contentment supports.

Feel free to use it, modify it, or don't use it, I don't care. Only please don't ask me to support your use of it.


### Installation

I would have liked the installation to perform unattended, but that feature on Umbraco v8 doesn't support the creation of new users. So you will need to clear out the value for the `Umbraco.Core.ConfigurationStatus` app-setting, to run a clean installation.

Once you have the clean Umbraco install ready, you can use the uSync dashboard (in the Settings section) to import the schema/settings and content.

If it doesn't work, let me know, we can collaborate to make it work.

### Contributions

Whilst I am open to contributions, please keep in mind that I am using this demo website is primarily for my own testing purposes, any suggestions would need to serve in my interest.
