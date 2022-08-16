


layer = app.activeLayer


function get_layer_by_name(name)
    for layer in app.activeDocument.layers do
        if layer.name == name then
            return layer
        end
    end
    return nil
end