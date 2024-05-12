public partial class APICfg
{
    // This folder contains configuration constants that are not meant to be published on GitHub
    // so this is the only file non ignored by the .gitignore file in the main folder.
    //
    // If you download the repository directly from GitHub you *will* receive errors when trying
    // to compile, because the current class won't feature the constant strings containing
    // the API urls needed for the HttpRequests to work.
    //
    // Once you add those to these class (either by adding those variables here or by creating
    //  a *.cfg.cs file of your own defining) everything will run smoothly.
    //
    // The urls you need are provided by running the following script for the backend project:
    //   firebase deploy --only functions
}