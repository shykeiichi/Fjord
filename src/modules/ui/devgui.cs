using System;
using System.Linq;
using Fjord.Modules.Graphics;
using Fjord.Modules.Input;
using Fjord.Modules.Mathf;

namespace Fjord.Modules.Ui
{
    [Obsolete("'devgui' module is deprecated and will not be maintained. Please use the 'fui' module.")]
    public static class devgui
    {
        private static string selected_input = "";

        private static V4 on_color = new V4(192, 175, 250, 255);
        private static V4 off_color = new V4(255, 255, 255, 255);
        private static V4 text_color = new V4(0, 0, 0, 255);

        public static void set_defaults(V4 on, V4 off, V4 text) {
            on_color = on;
            off_color = off;
            text_color = text;
        }

        public static void slider(V4 rect, ref int value, int max, V4 off, V4 on) {
            if (helpers.mouse_inside(rect, 2) && (mouse.pressed(0))) {
		        value = (int)((mouse.screen_position.x - rect.x) / ((float)rect.w / max));
            }

            value = Math.Clamp(value, 1, max);

            draw.rect(rect, off);
            rect.w = (int)(value * ((float)rect.w / (float)max)); 
            draw.rect(rect, on);
        }

        public static void slider(V4 rect, ref int value, int max) {
            if (helpers.mouse_inside(rect, 2) && (mouse.pressed(0))) {
		        value = (int)((mouse.screen_position.x - rect.x) / ((float)rect.w / max));
            }

            value = Math.Clamp(value, 1, max);

            draw.rect(rect, off_color);
            rect.w = (int)(value * ((float)rect.w / (float)max)); 
            draw.rect(rect, on_color);
        }

        public static void button(V4 rect, ref bool value, string font, string text, V4 off, V4 on, V4 text_color) {
            if (helpers.mouse_inside(rect, 2) && (mouse.just_pressed(mb.left))) 
                value = !value;

            if(!value)
                draw.rect(rect, off);
            else 
                draw.rect(rect, on);

            draw.text(new V2(rect.x + 5, rect.y + 5), font, rect.w - 10, text, text_color);
        }

        public static void button(V4 rect, ref bool value, string font, string text) {
            
            if (helpers.mouse_inside(rect, 2) && (mouse.just_pressed(mb.left))) 
                value = !value;

            if(!value)
                draw.rect(rect, off_color);
            else 
                draw.rect(rect, on_color);

            draw.text(new V2(rect.x + 5, rect.y + 5), font, rect.w - 10, text, text_color);
        }

        public static void num_input_box (V4 rect, string font, ref int value, string input_state, string id, V4 off, V4 on, V4 text_color) {
            if (helpers.mouse_inside(rect, 2) && (mouse.just_pressed(mb.left))) {
                selected_input = selected_input == id ? "" : id;
            }

            if(selected_input == id) {
                if(keyboard.just_pressed(key.backspace)) {
                    if(value.ToString().Length > 0)
                        Int32.TryParse(value.ToString().Substring(0, value.ToString().Length - 1), out value);
                } else if(keyboard.just_pressed(key.minus)) {
                    value = -value;
                } else {
                    try {
                        switch(keyboard.any_just_pressed()) {
                            case key.zero:
                                value = Convert.ToInt32(value.ToString() + "0");
                                break;
                            case key.one:
                                value = Convert.ToInt32(value.ToString() + "1");
                                break;
                            case key.two:
                                value = Convert.ToInt32(value.ToString() + "2");
                                break;
                            case key.three:
                                value = Convert.ToInt32(value.ToString() + "3");
                                break;
                            case key.four:
                                value = Convert.ToInt32(value.ToString() + "4");
                                break;
                            case key.five:
                                value = Convert.ToInt32(value.ToString() + "5");
                                break;
                            case key.six:
                                value = Convert.ToInt32(value.ToString() + "6");
                                break;
                            case key.seven:
                                value = Convert.ToInt32(value.ToString() + "7");
                                break;
                            case key.eight:
                                value = Convert.ToInt32(value.ToString() + "8");
                                break;
                            case key.nine:
                                value = Convert.ToInt32(value.ToString() + "9");
                                break;
                        }
                    } catch(OverflowException) {
                        
                    }
                }
            }

            if(selected_input != id)
                draw.rect(rect, off);
            else 
                draw.rect(rect, on);

            draw.text(new V2(rect.x + 5, rect.y + 5), font, rect.w - 10, value.ToString(), text_color);
        }

        public static void num_input_box (V4 rect, string font, ref int value, string input_state, string id) {
           if (helpers.mouse_inside(rect, 2) && (mouse.just_pressed(mb.left))) {
                selected_input = selected_input == id ? "" : id;
            }

            if(selected_input == id) {
                if(keyboard.just_pressed(key.backspace)) {
                    if(value.ToString().Length > 0)
                        Int32.TryParse(value.ToString().Substring(0, value.ToString().Length - 1), out value);
                } else if(keyboard.just_pressed(key.minus)) {
                    value = -value;
                } else {
                    try {
                        switch(keyboard.any_just_pressed()) {
                            case key.zero:
                                value = Convert.ToInt32(value.ToString() + "0");
                                break;
                            case key.one:
                                value = Convert.ToInt32(value.ToString() + "1");
                                break;
                            case key.two:
                                value = Convert.ToInt32(value.ToString() + "2");
                                break;
                            case key.three:
                                value = Convert.ToInt32(value.ToString() + "3");
                                break;
                            case key.four:
                                value = Convert.ToInt32(value.ToString() + "4");
                                break;
                            case key.five:
                                value = Convert.ToInt32(value.ToString() + "5");
                                break;
                            case key.six:
                                value = Convert.ToInt32(value.ToString() + "6");
                                break;
                            case key.seven:
                                value = Convert.ToInt32(value.ToString() + "7");
                                break;
                            case key.eight:
                                value = Convert.ToInt32(value.ToString() + "8");
                                break;
                            case key.nine:
                                value = Convert.ToInt32(value.ToString() + "9");
                                break;
                        }
                    } catch(OverflowException) {

                    }
                }
            }

            if(selected_input != id)
                draw.rect(rect, off_color);
            else 
                draw.rect(rect, on_color);

            draw.text(new V2(rect.x + 5, rect.y + 5), font, rect.w - 10, value.ToString(), text_color);
        }

        public static void input_box (V4 rect, string font, ref string value, string input_state, string id, string default_value, V4 off, V4 on, V4 text_color) {
            if(value == null) 
                return;

            if (helpers.mouse_inside(rect, 2) && (mouse.just_pressed(mb.left))) {
                selected_input = selected_input == id ? "" : id;

                if(keyboard.just_pressed(key.backspace)) {
                    if(!keyboard.pressed(key.lctrl))
                        if(value.Length > 0)
                            value = value.Substring(0, value.Length - 1);
                    else {
                        if(value.Length > 0) {
                            var valarr = value.Split(" ");
                            if(valarr.Any())
                                valarr = valarr.SkipLast(1).ToArray();
                            value = String.Join(" ", valarr);
                        }
                    }
                } else {
                    value += keyboard.keyboard_input();
                }
            }

            if(selected_input != id)
                draw.rect(rect, off);
            else 
                draw.rect(rect, on);

            draw.text(new V2(rect.x + 5, rect.y + 5), font, rect.w - 10, value != "" ? value : default_value, text_color);
        }

        public static void input_box (V4 rect, string font, ref string value, string input_state, string id, string default_value) {
            if(value == null) 
                return;

            if (helpers.mouse_inside(rect, 2) && (mouse.just_pressed(mb.left))) {
                selected_input = selected_input == id ? "" : id;
            }

            if(selected_input == id) {
                if(keyboard.just_pressed(key.backspace)) {
                    if(!keyboard.pressed(key.lctrl))
                        if(value.Length > 0)
                            value = value.Substring(0, value.Length - 1);
                    else {
                        if(value.Length > 0) {
                            var valarr = value.Split(" ");
                            if(valarr.Any())
                                valarr = valarr.SkipLast(1).ToArray();
                            value = String.Join(" ", valarr);
                        }
                    }
                } else {
                    value += keyboard.keyboard_input();
                }
            }

            if(selected_input != id)
                draw.rect(rect, off_color);
            else 
                draw.rect(rect, on_color);

            draw.text(new V2(rect.x + 5, rect.y + 5), font, rect.w - 10, value != "" ? value : default_value, text_color);
        }
    }
}