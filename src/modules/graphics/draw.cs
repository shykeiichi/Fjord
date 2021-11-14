using static SDL2.SDL;
using static SDL2.SDL_image;
using System;
using System.Threading;
using System.Threading.Tasks;
using Fjord.Modules.Camera;
using Fjord.Modules.Mathf;
using System.Runtime.InteropServices;

namespace Fjord.Modules.Graphics {
    public enum flip_type {
        none = 0,
        horizontal = 1,
        vertical = 2,
        both = 3
    }

    public static class draw {
        public static void rect(SDL_Rect rect, byte r, byte g, byte b, byte a, bool fill, bool relative = false) {
            SDL_Color old_color;
            rect.x -= (relative ? (int)camera.camera_position.x : 0);
            rect.y -= (relative ? (int)camera.camera_position.y : 0);

            SDL_GetRenderDrawColor(game.renderer, out old_color.r, out old_color.g, out old_color.b, out old_color.a);
            SDL_SetRenderDrawColor(game.renderer, r, g, b, a);

            if(fill) {
                SDL_RenderFillRect(game.renderer, ref rect);
            } else {
                SDL_RenderDrawRect(game.renderer, ref rect);
            }

            SDL_SetRenderDrawColor(game.renderer, old_color.r, old_color.g, old_color.b, old_color.a);
        }

        public static void round_rect(SDL_Rect rect, byte r, byte g, byte b, byte a, int border_radius, bool fill) {
            SDL_Color old_color;
            SDL_GetRenderDrawColor(game.renderer, out old_color.r, out old_color.g, out old_color.b, out old_color.a);
            SDL_SetRenderDrawColor(game.renderer, r, g, b, a);

            SDL_Rect horizontal;
            horizontal.x = rect.x;
            horizontal.y = rect.y + border_radius;
            horizontal.w = rect.w;
            horizontal.h = rect.h - border_radius * 2;

            SDL_Rect vertical1;
            vertical1.x = rect.x + border_radius;
            vertical1.y = rect.y;
            vertical1.w = rect.w - border_radius * 2;
            vertical1.h = border_radius;

            SDL_Rect vertical2;
            vertical2.x = rect.x + border_radius;
            vertical2.y = rect.y + rect.h - border_radius;
            vertical2.w = rect.w - border_radius * 2;
            vertical2.h = border_radius;

            if(fill) {

                SDL_RenderFillRect(game.renderer, ref horizontal);
                SDL_RenderFillRect(game.renderer, ref vertical1);
                SDL_RenderFillRect(game.renderer, ref vertical2);

                draw.quarter(rect.x + border_radius, rect.y + border_radius, border_radius, r, g, b, a, 1);
                draw.quarter(rect.x + rect.w - border_radius - 1, rect.y + border_radius, border_radius, r, g, b, a, 2);
                draw.quarter(rect.x + rect.w - border_radius - 1, rect.y + rect.h - border_radius - 1, border_radius, r, g, b, a, 3);
                draw.quarter(rect.x + border_radius, rect.y + rect.h - border_radius - 1, border_radius, r, g, b, a, 4);

            } else {
                SDL_RenderDrawRect(game.renderer, ref rect);
            }

            SDL_SetRenderDrawColor(game.renderer, old_color.r, old_color.g, old_color.b, old_color.a);
        }

        public static void circle(int x, int y, int radius, byte r, byte g, byte b, byte a){
            SDL_Color oldcolor;
            SDL_GetRenderDrawColor(game.renderer, out oldcolor.r, out oldcolor.g, out oldcolor.b, out oldcolor.a);
            SDL_SetRenderDrawColor(game.renderer, r, g, b, a);
            for (int w = 0; w < radius * 2; w++)
            {
                for (int h = 0; h < radius * 2; h++)
                {
                    int dx = radius - w; // horizontal offset
                    int dy = radius - h; // vertical offset
                    if ((dx*dx + dy*dy) <= (radius * radius))
                    {
                        SDL_RenderDrawPoint(game.renderer, x + dx, y + dy);
                    }
                }
            }
            SDL_SetRenderDrawColor(game.renderer, oldcolor.r, oldcolor.g, oldcolor.b, oldcolor.a);
        }  

        public static void quarter(int x, int y, int radius, byte r, byte g, byte b, byte a, int side){
            SDL_Color oldcolor;
            SDL_GetRenderDrawColor(game.renderer, out oldcolor.r, out oldcolor.g, out oldcolor.b, out oldcolor.a);
            SDL_SetRenderDrawColor(game.renderer, r, g, b, a);
            for (int w = 0; w < radius * 2; w++)
            {
                for (int h = 0; h < radius * 2; h++)
                {
                    int dx = radius - w; // horizontal offset
                    int dy = radius - h; // vertical offset
                    if ((dx*dx + dy*dy) <= (radius * radius))
                    {
                        if(w > radius && h > radius && side == 1) {
                            SDL_RenderDrawPoint(game.renderer, x + dx, y + dy);
                        }
                        else if(w < radius && h > radius && side == 2) {
                            SDL_RenderDrawPoint(game.renderer, x + dx, y + dy);
                        }
                        else if(w < radius && h < radius && side == 3) {
                            SDL_RenderDrawPoint(game.renderer, x + dx, y + dy);
                        }
                        else if(w > radius && h < radius && side == 4) {
                            SDL_RenderDrawPoint(game.renderer, x + dx, y + dy);
                        }
                    }
                }
            }
            SDL_SetRenderDrawColor(game.renderer, oldcolor.r, oldcolor.g, oldcolor.b, oldcolor.a);
        } 
    
        public static void texture(IntPtr texture, int x, int y, double angle, byte alpha=255, bool relative=false, flip_type flip=flip_type.none) {
            SDL_Point size;
            uint format;
            int access;
            SDL_QueryTexture(texture, out format, out access, out size.x, out size.y);

            SDL_Rect src, dest;

            SDL_Point center;
            center.x = size.x / 2;
            center.y = size.y / 2;

            src.x = src.y = 0;
            src.w = size.x;
            src.h = size.y;

            // dest.x = x - size.x / 2 - (relative ? (int)camera.camera_position.X : 0);
            // dest.y = y - size.y / 2 - (relative ? (int)camera.camera_position.Y : 0);
            dest.x = x - (relative ? (int)camera.camera_position.x : 0);
            dest.y = y - (relative ? (int)camera.camera_position.y : 0);
            dest.w = size.x;
            dest.h = size.y;
            
            SDL_RendererFlip flip_sdl = SDL_RendererFlip.SDL_FLIP_NONE; 
            
            if(flip == flip_type.both) {
                flip_sdl = SDL_RendererFlip.SDL_FLIP_HORIZONTAL | SDL_RendererFlip.SDL_FLIP_VERTICAL;
            } else if(flip == flip_type.horizontal) {
                flip_sdl = SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
            } else if(flip == flip_type.vertical) {
                flip_sdl = SDL_RendererFlip.SDL_FLIP_VERTICAL;
            }

            SDL_SetTextureAlphaMod(texture, alpha);

            SDL_RenderCopyEx(game.renderer, texture, ref src, ref dest, angle, ref center, flip_sdl);
        }

        public static void texture_ext(IntPtr texture, int x, int y, double angle, double x_scale, double y_scale, byte alpha=255, bool relative=false, flip_type flip=flip_type.none) {
            uint f;
            int a, w, h;
            SDL_Point origin = new SDL_Point(0, 0);

            SDL_QueryTexture(texture, out f, out a, out w, out h);

            SDL_Rect src = new SDL_Rect(0, 0, w, h);

            SDL_Rect dest = new SDL_Rect(x - (relative ? (int)camera.camera_position.x : 0), y - (relative ? (int)camera.camera_position.y : 0), (int)(w * x_scale), (int)(h * y_scale));

            SDL_RendererFlip flip_sdl = SDL_RendererFlip.SDL_FLIP_NONE; 
            
            if(flip == flip_type.both) {
                flip_sdl = SDL_RendererFlip.SDL_FLIP_HORIZONTAL | SDL_RendererFlip.SDL_FLIP_VERTICAL;
            } else if(flip == flip_type.horizontal) {
                flip_sdl = SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
            } else if(flip == flip_type.vertical) {
                flip_sdl = SDL_RendererFlip.SDL_FLIP_VERTICAL;
            }

            SDL_SetTextureAlphaMod(texture, alpha);

            SDL_RenderCopyEx(game.renderer, texture, ref src, ref dest, 0, ref origin, flip_sdl);
        }

        public static void texture_atlas(IntPtr texture_atlas, int atlas_x, int atlas_y, int atlas_w, int atlas_h, int x, int y, double angle, int dest_w, int dest_h, SDL_Point origin_, bool relative=false, flip_type flip=flip_type.none) {

            SDL_Rect src_rect = new SDL_Rect(atlas_x, atlas_y, atlas_w, atlas_h);
            SDL_Rect dest_rect = new SDL_Rect(x - (relative ? (int)camera.camera_position.x : 0), y - (relative ? (int)camera.camera_position.y : 0), dest_w, dest_h);

            SDL_RendererFlip flip_sdl = SDL_RendererFlip.SDL_FLIP_NONE; 
            
            if(flip == flip_type.both) {
                flip_sdl = SDL_RendererFlip.SDL_FLIP_HORIZONTAL | SDL_RendererFlip.SDL_FLIP_VERTICAL;
            } else if(flip == flip_type.horizontal) {
                flip_sdl = SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
            } else if(flip == flip_type.vertical) {
                flip_sdl = SDL_RendererFlip.SDL_FLIP_VERTICAL;
            }

            SDL_RenderCopyEx(game.renderer, texture_atlas, ref src_rect, ref dest_rect, angle, ref origin_, flip_sdl);
        }

        public static void line(int x, int y, int x2, int y2, byte r, byte g, byte b, byte a) {
            SDL_Color old_color;
            SDL_GetRenderDrawColor(game.renderer, out old_color.r, out old_color.g, out old_color.b, out old_color.a);
            SDL_SetRenderDrawColor(game.renderer, r, g, b, a);

            SDL_RenderDrawLine(game.renderer, x, y, x2, y2);

            SDL_SetRenderDrawColor(game.renderer, old_color.r, old_color.g, old_color.b, old_color.a);
        }

        public static void text(int x, int y, string font, int font_size, string text, byte r=255, byte g=255, byte b=255, byte a=255) {
            IntPtr texture; 
            font_handler.get_texture(text, font, out texture, 0, 0, r, g, b, a);

            SDL_Rect src;
            src.x = src.y = 0;

            double scale = (double)font_size / (double)font_handler.get_font_size(font);

            draw.texture_ext(texture, x, y, 0, scale, scale);
        }
    }
}