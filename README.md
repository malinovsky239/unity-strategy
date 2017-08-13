This demo project features various elements of a real-time strategy.

You can watch demo video [here](https://youtu.be/JppXaf6VvQA). In addition to gameplay, this video contains short explanation of the algorithm behind units' formation change.

Some interesting points in the source code:
* Formation change (featuring search for the perfect matching with the additional criterion: weight of the heaviest edge should be as small as possible): [code](https://github.com/malinovsky239/unity-strategy/blob/master/Assets/Scripts/SelectionController.cs#L32).
* Programmatically created sector-shaped mesh representing unit's field of view: [code](https://github.com/malinovsky239/unity-strategy/blob/master/Assets/Scripts/FieldOfView.cs#L25).
* Smooth switch between "strategy-style" and FPS-like camera: [code](https://github.com/malinovsky239/unity-strategy/blob/master/Assets/Scripts/CameraMovement.cs#L27).
