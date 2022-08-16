
filename = ""

print(app.activeSprite.layers)
for i, layer in ipairs(app.activeSprite.layers) do
    print(layer.name)
end

function get_layer_by_name(name)
    for i, layer in ipairs(app.activeSprite.layers) do
        if layer.name == name then
            return layer
        end
    end
    return nil
end

function hide_all_in_group(group)
    for i, sublayer in ipairs(group.layers) do
        sublayer.isVisible = false
    end
end

local dlg = Dialog()
dlg:file{ id='id_file',
          label='Test',
          title="Select target folder.",
          open=false,
          save=true,
          filename='icon.png',
          onchange=function(ev)
            filename = dlg.data['id_file']
            filename = string.sub(filename, 1, string.find(filename, "[.]")-1)
            print(filename)
          end
}
dlg:button{ id='id_ok',
            onclick=function(ev)
                group_base = get_layer_by_name("base")
                group_shadow = get_layer_by_name("sombra")

                for i, layer in ipairs(group_base.layers) do
                    print('Saving ' .. layer.name)
                    hide_all_in_group(group_base)
                    hide_all_in_group(group_shadow)
                    group_base.layers[i].isVisible = true
                    group_shadow.layers[i].isVisible = true
                    app.command.SaveFileAs{
                        ui=false,
                        filename=filename..i..".png",
                    }
                end
            end
}
dlg:show{wait=true}
