# Known issues of the App
At the moment, there are several issues that are known. Most of them are the result of missing implementations or bugs in the current version of `.NET MAUI`.
The intend of this file is to create a list of all known issues. They may be fixed automatically when the missing implementation is added to `MAUI` or the bugs are
fixed. However they may also not be resolved by themselves.

## The currently known issues
* Focus handling for `Entry` is currently not implemented. Therefore the validation of the `Entry`s content may not get triggered
* `CornerRadius` is currently not working for `ImageButton`, so all `ImageButton`s in the application are rects. As soon as this is fixed/implemented they will be round
* Changing the `BackgroundColor` of a `Button` or `ImageButton` removes the "ripple" effekt. This is why all `ImageButton`s do not have a white background
* `Modal`s are not the correct size and overlaying/removing the white background for the Android software navigationbar
* One can still interact with components that are overlayed with a modal, so it may be possible to accidentally hit a button in the service view when creating or editing a service
