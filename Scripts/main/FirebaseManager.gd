extends Node

func _ready():
	# Registers to authentication events
	Firebase.Auth.login_succeeded.connect(on_login_succeeded)
	Firebase.Auth.signup_succeeded.connect(on_signup_succeeded)
	Firebase.Auth.login_failed.connect(on_login_failed)
	Firebase.Auth.signup_failed.connect(on_signup_failed)
	Firebase.Auth.logged_out.connect(on_logout)
	
	if Firebase.Auth.check_auth_file():
		Firebase.Auth.load_auth()
		
	_update_view(Firebase.Auth.check_auth_file())
	pass
	
func _update_view(show_main):
	%LoginPanel.visible = !show_main
	%MainMenu.visible = show_main
	print_debug(show_main)
	pass

func _on_login_button_pressed():
	var email = %EmailField.text
	var password = %PasswordField.text
	Firebase.Auth.login_with_email_and_password(email, password)
	%StatusLabel.text = "LOGGING IN"
	pass

func _on_signup_button_pressed():
	print_debug("signup")
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
	_update_view(true)
	pass

func on_signup_succeeded(auth):
	%StatusLabel.text = "SIGN UP OK!"
	%ErrorLabel.text = ""
	Firebase.Auth.save_auth(auth)
	_update_view(true)
	pass

func on_login_failed(error_code, message):
	%StatusLabel.text = "LOGIN FAILED"
	%ErrorLabel.text = message
	pass

func on_signup_failed(error_code, message):
	%StatusLabel.text = "SIGN UP FAILED"
	%ErrorLabel.text = message
	pass

func on_logout():
	%StatusLabel.text = "NOT LOGGED IN"
	%ErrorLabel.text = ""
	_update_view(false)
	print_debug(Firebase.Auth.check_auth_file())
	pass
