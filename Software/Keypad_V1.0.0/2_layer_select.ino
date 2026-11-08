void layer_select(){

  int layer_state = digitalRead(layer_butt_pin);

  if ((layer_state == LOW) && ((millis() - last_layer_db) > layer_db_delay)) {
    
    last_layer_db = millis();

    selected_layer += 1;
    key_layer_update();

  }
}
