extends Node

var player_id = null

func _ready():
	# Registers to authentication events
	Firebase.Auth.login_succeeded.connect(on_login_succeeded)
	Firebase.Auth.signup_succeeded.connect(on_signup_succeeded)
	Firebase.Auth.login_failed.connect(on_login_failed)
	Firebase.Auth.signup_failed.connect(on_signup_failed)
	Firebase.Auth.logged_out.connect(on_logout)
	
	if Firebase.Auth.check_auth_file():
		Firebase.Auth.load_auth()
		_update_status(true)
	else:
		_update_status(false)
	pass

func _update_status(connected):
	if connected:
		player_id = Firebase.Auth.auth.localid
	else:
		player_id = null
	pass

func _on_login_button_pressed():
	var email = %EmailField.text
	var password = %PasswordField.text
	Firebase.Auth.login_with_email_and_password(email, password)
	%StatusLabel.text = "LOGGING IN"
	pass

func _on_signup_button_pressed():
	var email = %EmailField.text
	var password = %PasswordField.text
	Firebase.Auth.signup_with_email_and_password(email, password)
	%StatusLabel.text = "SIGNING UP"
	pass
	
func _on_logout_button_pressed():
	Firebase.Auth.logout()
	pass

func on_login_succeeded(auth):
	%StatusLabel.text = "LOGGED IN"
	%ErrorLabel.text = ""
	Firebase.Auth.save_auth(auth)
	_update_status(true)
	pass

func on_signup_succeeded(auth):
	%StatusLabel.text = "SIGN UP OK!"
	%ErrorLabel.text = ""
	Firebase.Auth.save_auth(auth)
	_update_status(true)
	pass

func on_login_failed(_error_code, message):
	%StatusLabel.text = "LOGIN FAILED"
	%ErrorLabel.text = message
	pass

func on_signup_failed(_error_code, message):
	%StatusLabel.text = "SIGN UP FAILED"
	%ErrorLabel.text = message
	pass

func on_logout():
	%StatusLabel.text = "NOT LOGGED IN"
	%ErrorLabel.text = ""
	_update_status(false)
	pass
