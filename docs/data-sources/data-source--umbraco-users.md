<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Data Sources

#### Umbraco Users

This data-source uses the Umbraco users to populate the items of a compatible editor, e.g. [Data List](../editors/data-list.md).


##### How to configure the editor?

The configuration of the Umbraco Users data-source has one option, to select a **User Group** (role) to filter the users that are listed. If left empty, then all users will be listed.

![image](https://user-images.githubusercontent.com/9142936/196560717-56702f0b-fa70-4392-ad0d-1d02c254c386.png)

![image](https://user-images.githubusercontent.com/9142936/196560747-5b89fde6-5b4c-42d8-86c3-3f3690e78842.png)

![image](https://user-images.githubusercontent.com/9142936/196560769-547b95ea-1223-470b-87c5-400c725c58af.png)

##### What is the value's object-type?

The value for the Umbraco Users data-source item is an [`IUser`](https://github.com/umbraco/Umbraco-CMS/blob/release-9.0.0/src/Umbraco.Core/Models/Membership/IUser.cs) object-type. Typically, this will be a concrete class of [`User`](https://github.com/umbraco/Umbraco-CMS/blob/release-9.0.0/src/Umbraco.Core/Models/Membership/User.cs), _(unless if you are doing advanced user group management?)._

Depending on the `List editor` used, this may be wrapped in a `List<IUser>`.

![image](https://user-images.githubusercontent.com/9142936/196560793-9e240125-68c1-4c07-bb52-ef37050c7ab9.png)
