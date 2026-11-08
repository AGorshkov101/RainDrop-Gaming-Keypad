void joy_check(byte joy_x_pin, byte joy_y_pin, int x_zero, int y_zero, char* joyCurrentState, char* joyPreviusState){
  
  // Get joystick position 
  float JoyXVal = analogRead(joy_x_pin) - x_zero;
  float JoyYVal = analogRead(joy_y_pin) - y_zero;

  // Calculate angle and radius 
  float joyMagnitude = sqrt((JoyXVal*JoyXVal) + (JoyYVal*JoyYVal));
  float joyAngle = atan2(JoyXVal, JoyYVal) * (180/PI);

  // Rotate X and Y to account for ergonomics
  // Edit "joyAdjustment" value. Adjust between -45 to 45. Positive number rotates joystick clockwise   
  const int joyAdjustment = savedJoyAdjustment;
  joyAngle = joyAngle - joyAdjustment;

  // Convert to 0 to 360 deg 
  if ( joyAngle < 0){
    joyAngle = joyAngle + 360;
  }

  // Determine if joy is active and set its state
  const int trigger = 200; 
  if (joyMagnitude > trigger){

    if ((joyAngle <= 45) or (joyAngle > 315)){
      *joyCurrentState = joy_matrix[selected_layer][2];
      //Serial.println("Forward");
    }
    else if ((joyAngle <= 135) && (joyAngle > 45)){
      *joyCurrentState = joy_matrix[selected_layer][1];
      //Serial.println("Down");
    }
    else if ((joyAngle <= 225) && (joyAngle > 135)){
      *joyCurrentState = joy_matrix[selected_layer][3];
      //Serial.println("Back");
    }
    else {
      *joyCurrentState = joy_matrix[selected_layer][0];
      //Serial.println("Up");
    }
  }
  else{
    *joyCurrentState = '\0';
  }

  // Perform action based on state 
  if (*joyCurrentState != *joyPreviusState){
    Keyboard.release(*joyPreviusState);
    Keyboard.press(*joyCurrentState);
    *joyPreviusState = *joyCurrentState;    
  }

//  Serial.print("X: ");
//  Serial.println(JoyXVal);
//
//  Serial.print("Y: ");
//  Serial.println(JoyYVal);  
//  
//  Serial.print("Magnitude: ");
//  Serial.println(joyMagnitude);
//
//  Serial.print("Angle: ");
//  Serial.println(joyAngle);
//  delay(250);

}
