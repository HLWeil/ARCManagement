from arc_management import ExampleModel, get_runtime_name


def test_attached_members_and_copy():
    model = ExampleModel("example", 2)
    assert model.Name == "example"
    copy = model.Copy()
    model.Name = "changed"
    model.Increment(3)
    assert model.Value == 5
    assert copy.Name == "example"
    assert copy.Value == 2
    model.Value = 0
    assert model.Value == 0


def test_runtime_name():
    assert get_runtime_name() == "Python"
