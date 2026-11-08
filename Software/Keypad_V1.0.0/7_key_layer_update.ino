
void key_layer_update(){

  if (selected_layer > total_layers - 1){
    selected_layer = 0;
  }
  current_kpd = kpd_matrix[selected_layer];
  current_butt = butt_matrix[selected_layer];
  
  /// LED LAYER INDICATION
  digitalWrite(led1_pin, led_state[selected_layer][0]);
  digitalWrite(led2_pin, led_state[selected_layer][1]);
  digitalWrite(led3_pin, led_state[selected_layer][2]);
  digitalWrite(led4_pin, led_state[selected_layer][3]);

  // save layer
  EEPROM.write(layer_adrs, selected_layer);
  //Serial.println(selected_layer); // tell PC the current player
}
