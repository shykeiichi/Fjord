using Fjord.Modules.Input;
using Fjord.Modules.Ui;
using Fjord.Modules.Debug;
using Fjord.Modules.Graphics;
using Fjord.Modules.Camera;
using Fjord.Modules.Game;
using Fjord.Modules.Misc;
using Fjord.Modules.Mathf;
using System.Collections.Generic;
using static SDL2.SDL;
using System;
using System.Numerics;
using Newtonsoft.Json;

namespace Fjord.Game {
    public class tilemap_editor : scene {
        
        float zoom = 1;
        V2 pos;

        bool load_tex = false;
        string load_texture_string = "";

        bool load_tex_button = false;

        bool export_file = false;
        string export_file_string = "";

        bool export_file_button = false;

        string asset_pack = "general";
        bool change_asset_pack = false;

        tilemap Tilemap = new tilemap(20, 20, 8, 8, 8, 8);
        IntPtr atlas;

        int grid_x, grid_y, grid_x_end, grid_y_end;

        V2 selected_tile = new V2(-1, -1);

        public override void on_load() {
            if(!scene_handler.get_scene("tilemap_editor")) {
                scene_handler.add_scene("tilemap_editor", new tilemap_editor());
                scene_handler.load_scene("tilemap_editor");
            }

            game_manager.set_asset_pack("MiniJam88");
            Tilemap.atlas_str = "atlas.png";
            atlas = texture_handler.load_texture("atlas.png", game_manager.renderer);

            game_manager.set_asset_pack("general");
            font_handler.load_font("font", "FiraCode", 22);
        }

        public override void update() {
            if(input.get_key_just_pressed(input.key_r)) {
                if(input.get_key_pressed(input.key_lshift) && input.get_key_pressed(input.key_lctrl)) {
                    game_manager.set_asset_pack("MiniJam88");
                    atlas = texture_handler.load_texture("atlas.png", game_manager.renderer);
                }
            }

            if(mouse.scrolling(0)) {
                zoom += 0.1f;
            } else if(mouse.scrolling(1)) {
                zoom -= 0.1f;
            }

            int move_sp = 4;
            if(input.get_key_pressed(input.key_w, "general")) {
                pos.y -= move_sp;
            } else if(input.get_key_pressed(input.key_s, "general")) {
                pos.y += move_sp;
            }

            if(input.get_key_pressed(input.key_a, "general")) {
                pos.x-= move_sp;
            } else if(input.get_key_pressed(input.key_d, "general")) {
                pos.x+= move_sp;
            }   

            camera.set_viewport(pos.x, pos.y);   

            if(Mathf.mouse_inside(470, 10, 200, 30) && mouse.button_just_pressed(0)) {
                change_asset_pack = !change_asset_pack;
            }  else if(!Mathf.mouse_inside(470, 10, 200, 30) && mouse.button_just_pressed(0)) {
                change_asset_pack = false;
            }

            if(change_asset_pack) {
                input.set_input_state("set_asset_pack");
            } else if(input.input_state == "set_asset_pack") {
                input.set_input_state("general");
            }

            if(Mathf.mouse_inside(260, 100, 200, 30) && mouse.button_just_pressed(0)) {
                export_file = !export_file;
            } else if(!Mathf.mouse_inside(260, 100, 200, 30) && mouse.button_just_pressed(0)) {
                export_file = false;
            }

            if(export_file) {
                input.set_input_state("export_tilemap");
            } else if(input.input_state == "export_tilemap") {
                input.set_input_state("general");
            }

            if(export_file_button) {
                export_file_button = false;
                export_file = false;

                var _export_output = new tilemap(Tilemap.w, Tilemap.h, Tilemap.grid_w, Tilemap.grid_h, Tilemap.atlas_gridw, Tilemap.atlas_gridh) {
                    atlas_str = Tilemap.atlas_str,
                    map = Tilemap.map,
                    collision_map = Tilemap.collision_map,
                    asset_pack = asset_pack
                }; 

                var json_string = JsonConvert.SerializeObject(_export_output);

                string full_path = game_manager.executable_path + "\\resources\\" + asset_pack + "\\data\\tilemaps\\" + export_file_string;
                System.IO.File.WriteAllText(full_path, json_string);
            }

            if(Mathf.mouse_inside(260, 10, 200, 30) && mouse.button_just_pressed(0)) {
                load_tex = !load_tex;
            } else if(!Mathf.mouse_inside(260, 10, 200, 30) && mouse.button_just_pressed(0)) {
                load_tex = false;
            }

            if(load_tex) {
                input.set_input_state("load_texture");
            } else if(input.input_state == "load_texture") {
                input.set_input_state("general");
            }

            if(load_tex_button) {
                load_tex_button = false;
                load_tex = false;

                var full_path = game_manager.executable_path + "\\resources\\MiniJam88\\data\\tilemaps\\" + load_texture_string;
                var file = System.IO.File.ReadAllText(full_path);

                Tilemap = JsonConvert.DeserializeObject<tilemap>(file);
            }

            grid_x = (int)(Tilemap.grid_w * zoom) - (int)camera.camera_position.x - (int)(Tilemap.grid_w * zoom);
            grid_y = (int)(Tilemap.grid_h * zoom) - (int)camera.camera_position.y - (int)(Tilemap.grid_h * zoom);
            grid_x_end = grid_x + Tilemap.w * (int)(Tilemap.grid_w * zoom);
            grid_y_end = grid_y + Tilemap.h * (int)(Tilemap.grid_h * zoom);

            if(mouse.button_pressed(1)) {
                var x_ = (mouse.x- grid_x);
                var y_ = (mouse.y - grid_y);
                var w_ = Tilemap.grid_w * zoom;
                var h_ = Tilemap.grid_h * zoom;
    
                x_ = x_ / (int)w_;
                y_ = y_ / (int)h_;

                if(x_ < Tilemap.w && x_ > -1 && y_ < Tilemap.h && y_ > -1) {
                    Tilemap.collision_map[tilemap_funcs.create_pos(x_, y_)] = false;
                }                
            }

            if(mouse.button_pressed(0)) {

                var x_ = (mouse.x- grid_x);
                var y_ = (mouse.y - grid_y);
                var w_ = Tilemap.grid_w * zoom;
                var h_ = Tilemap.grid_h * zoom;
    
                x_ = x_ / (int)w_;
                y_ = y_ / (int)h_;

                if(x_ < Tilemap.w && x_ > -1 && y_ < Tilemap.h && y_ > -1) {
                    if(!input.get_key_pressed(input.key_lshift))
                        Tilemap.map[tilemap_funcs.create_pos(x_, y_)] = selected_tile;
                    if(input.get_key_pressed(input.key_lctrl) && !input.get_key_pressed(input.key_lshift)) {
                        Tilemap.map[tilemap_funcs.create_pos(x_ - 1, y_ - 1)] = selected_tile;
                        Tilemap.map[tilemap_funcs.create_pos(x_ - 1, y_)] = selected_tile;
                        Tilemap.map[tilemap_funcs.create_pos(x_ - 1, y_ + 1)] = selected_tile;
                        Tilemap.map[tilemap_funcs.create_pos(x_, y_ - 1)] = selected_tile;
                        Tilemap.map[tilemap_funcs.create_pos(x_, y_)] = selected_tile;
                        Tilemap.map[tilemap_funcs.create_pos(x_, y_ + 1)] = selected_tile;
                        Tilemap.map[tilemap_funcs.create_pos(x_ + 1, y_ - 1)] = selected_tile;
                        Tilemap.map[tilemap_funcs.create_pos(x_ + 1, y_)] = selected_tile;
                        Tilemap.map[tilemap_funcs.create_pos(x_ + 1, y_ + 1)] = selected_tile;
                    }
                    if(input.get_key_pressed(input.key_lshift))
                        Tilemap.collision_map[tilemap_funcs.create_pos(x_, y_)] = true; 
                    if(input.get_key_pressed(input.key_lctrl) && input.get_key_pressed(input.key_lshift)) {
                        Tilemap.collision_map[tilemap_funcs.create_pos(x_ - 1, y_ - 1)] = true;
                        Tilemap.collision_map[tilemap_funcs.create_pos(x_ - 1, y_)] = true;
                        Tilemap.collision_map[tilemap_funcs.create_pos(x_ - 1, y_ + 1)] = true;
                        Tilemap.collision_map[tilemap_funcs.create_pos(x_, y_ - 1)] = true;
                        Tilemap.collision_map[tilemap_funcs.create_pos(x_, y_)] = true;
                        Tilemap.collision_map[tilemap_funcs.create_pos(x_, y_ + 1)] = true;
                        Tilemap.collision_map[tilemap_funcs.create_pos(x_ + 1, y_ - 1)] = true;
                        Tilemap.collision_map[tilemap_funcs.create_pos(x_ + 1, y_)] = true;
                        Tilemap.collision_map[tilemap_funcs.create_pos(x_ + 1, y_ + 1)] = true;
                    }
                }
            }

            if(Math.Round(mouse.x/ 58f) < 4 && Math.Round(mouse.y / 58f) < 10 && mouse.button_just_pressed(0)) {
                selected_tile.x = (int)Math.Round((10f + mouse.x) / 58f) - 1;
                selected_tile.y = (int)Math.Round((10f + mouse.y) / 58f) - 1;
            }
        }

        public override void render() {
            for(var i = 0; i < Tilemap.w; i++) {
                for(var j = 0; j < Tilemap.h; j++) {
                    SDL_Rect rect;
                    rect.x= i * (int)(Tilemap.grid_w * zoom) + (grid_x);
                    rect.y = j * (int)(Tilemap.grid_h * zoom) + (grid_y);
                    rect.w = (int)(Tilemap.grid_w * zoom);
                    rect.h = (int)(Tilemap.grid_h * zoom);
                    draw.rect(game_manager.renderer, rect, 255, 255, 255, 255, false);

                    draw.texture_atlas(game_manager.renderer, atlas, (int)Tilemap.map[tilemap_funcs.create_pos(i, j)].x * Tilemap.atlas_gridw, (int)Tilemap.map[tilemap_funcs.create_pos(i, j)].y * Tilemap.atlas_gridh, 8, 8, rect.x, rect.y, 0, rect.w, rect.h, new SDL_Point(0, 0), false, draw_origin.CENTER, flip_type.none);
                    if(Tilemap.collision_map[tilemap_funcs.create_pos(i, j)])
                        draw.rect(game_manager.renderer, rect, 255, 0, 0, 50, true, false);
                }
            }

            // Draws background to ui

            SDL_Rect rect1;
            rect1.x= 0;
            rect1.y = 0;
            rect1.w = 250;
            rect1.h = 1280;
            draw.rect(game_manager.renderer, rect1, 0, 0, 0, 200, true);

            // Draws atlas

            draw.texture_ext(game_manager.renderer, atlas, 10, 10, 0, 230, 575, false);

            if(Math.Round(mouse.x/ 58f) < 4 && Math.Round(mouse.y / 58f) < 10)
                draw.rect(game_manager.renderer, new SDL_Rect(10 + (int)(Math.Round((mouse.x- 29) / 58f) * 58f), 10 + (int)(Math.Round((mouse.y - 29) / 58f) * 58f), 58, 58), 0, 255, 0, 255, false);

            draw.rect(game_manager.renderer, new SDL_Rect(10 + (int)selected_tile.x* 58, 10 + (int)selected_tile.y * 58, 58, 58), 119, 172, 241, 100, true);

            // Draws ui

            zgui.input_box(260, 10, 200, 30, "font", ref load_texture_string, "texture path", "load_texture");
            zgui.button(260, 50, 80, 30, ref load_tex_button, "font", "Load");

            zgui.input_box(260, 100, 200, 30, "font", ref export_file_string, "export path", "export_tilemap");
            zgui.button(260, 140, 100, 30, ref export_file_button, "font", "Export");

            zgui.input_box(470, 10, 200, 30, "font", ref asset_pack, "", "set_asset_pack");
        }
    }
}