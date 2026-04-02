import 'package:flutter/material.dart';
import 'package:frontend_qlte/models/auth/forgot_password_request_model.dart';
import 'package:frontend_qlte/models/auth/verify_forgot_password_request_model.dart';
import 'package:frontend_qlte/models/auth/reset_password_request_model.dart';
import 'package:frontend_qlte/models/common/api_response_model.dart';
import 'package:frontend_qlte/services/auth_service.dart';

class ForgotPasswordScreen extends StatefulWidget {
  const ForgotPasswordScreen({super.key});

  @override
  State<ForgotPasswordScreen> createState() => _ForgotPasswordScreenState();
}

class _ForgotPasswordScreenState extends State<ForgotPasswordScreen> {
  final _emailController = TextEditingController();
  final _otpController = TextEditingController();
  final _newPasswordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();

  bool _isLoading = false;
  String? _errorMessage;
  String? _successMessage;
  int _step = 1; // 1: gửi email, 2: xác OTP, 3: đổi mật khẩu

  void _showMessage(ApiResponseModel res) {
    setState(() {
      if (res.status) {
        _successMessage = res.msg;
        _errorMessage = null;
      } else {
        _errorMessage = res.msg;
        _successMessage = null;
      }
    });
  }

  Future<void> _sendForgotPassword() async {
    setState(() {
      _isLoading = true;
      _errorMessage = null;
      _successMessage = null;
    });

    final res = await AuthService.ForgotPassword(
      ForgotPasswordRequestModel(
        email: _emailController.text.trim(),
        type: "ForgotPassword",
      ),
    );

    _showMessage(res);

    if (res.status) setState(() => _step = 2);

    setState(() => _isLoading = false);
  }

  Future<void> _verifyOtp() async {
    setState(() {
      _isLoading = true;
      _errorMessage = null;
      _successMessage = null;
    });

    final res = await AuthService.VerifyForgotPassword(
      VerifyForgotPasswordRequestModel(
        email: _emailController.text.trim(),
        otp: _otpController.text.trim(),
        type: "ForgotPassword",
      ),
    );

    _showMessage(res);

    if (res.status) setState(() => _step = 3);

    setState(() => _isLoading = false);
  }

  Future<void> _resetPassword() async {
    if (_newPasswordController.text != _confirmPasswordController.text) {
      setState(() => _errorMessage = "Mật khẩu xác nhận không khớp!");
      return;
    }

    setState(() {
      _isLoading = true;
      _errorMessage = null;
      _successMessage = null;
    });

    final res = await AuthService.ResetPassword(
      ResetPasswordRequestModel(
        email: _emailController.text.trim(),
        otp: _otpController.text.trim(),
        type: "ForgotPassword",
        newPassword: _newPasswordController.text.trim(),
        confirmNewPassword: _confirmPasswordController.text.trim(),
      ),
    );

    _showMessage(res);

    if (res.status) {
      Future.delayed(const Duration(seconds: 2), () {
        if (mounted) Navigator.pop(context);
      });
    }

    setState(() => _isLoading = false);
  }

  Widget _buildEmailStep() {
    return Column(
      children: [
        TextField(
          controller: _emailController,
          decoration: const InputDecoration(
            labelText: "Email",
            prefixIcon: Icon(Icons.email_outlined),
            border: OutlineInputBorder(),
          ),
        ),
        const SizedBox(height: 20),
        ElevatedButton(
          onPressed: _isLoading ? null : _sendForgotPassword,
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.blueAccent,
            minimumSize: const Size(double.infinity, 50),
          ),
          child: _isLoading
              ? const CircularProgressIndicator(color: Colors.white)
              : const Text("Gửi mã OTP", style: TextStyle(fontSize: 18)),
        ),
      ],
    );
  }

  Widget _buildOtpStep() {
    return Column(
      children: [
        TextField(
          controller: _otpController,
          decoration: const InputDecoration(
            labelText: "Nhập mã OTP",
            prefixIcon: Icon(Icons.lock_outline),
            border: OutlineInputBorder(),
          ),
        ),
        const SizedBox(height: 20),
        ElevatedButton(
          onPressed: _isLoading ? null : _verifyOtp,
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.blueAccent,
            minimumSize: const Size(double.infinity, 50),
          ),
          child: _isLoading
              ? const CircularProgressIndicator(color: Colors.white)
              : const Text("Xác nhận OTP", style: TextStyle(fontSize: 18)),
        ),
      ],
    );
  }

  Widget _buildResetPasswordStep() {
    return Column(
      children: [
        TextField(
          controller: _newPasswordController,
          obscureText: true,
          decoration: const InputDecoration(
            labelText: "Mật khẩu mới",
            prefixIcon: Icon(Icons.lock_reset),
            border: OutlineInputBorder(),
          ),
        ),
        const SizedBox(height: 16),
        TextField(
          controller: _confirmPasswordController,
          obscureText: true,
          decoration: const InputDecoration(
            labelText: "Xác nhận mật khẩu mới",
            prefixIcon: Icon(Icons.check_circle_outline),
            border: OutlineInputBorder(),
          ),
        ),
        const SizedBox(height: 20),
        ElevatedButton(
          onPressed: _isLoading ? null : _resetPassword,
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.blueAccent,
            minimumSize: const Size(double.infinity, 50),
          ),
          child: _isLoading
              ? const CircularProgressIndicator(color: Colors.white)
              : const Text("Đặt lại mật khẩu", style: TextStyle(fontSize: 18)),
        ),
      ],
    );
  }

  Widget _buildStepContent() {
    switch (_step) {
      case 1:
        return _buildEmailStep();
      case 2:
        return _buildOtpStep();
      case 3:
        return _buildResetPasswordStep();
      default:
        return Container();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.blue.shade50,
      appBar: AppBar(
        title: const Text("Quên mật khẩu"),
        backgroundColor: Colors.blueAccent,
      ),
      body: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          children: [
            const Icon(Icons.lock_reset, size: 80, color: Colors.blueAccent),
            const SizedBox(height: 20),
            if (_errorMessage != null)
              Text(_errorMessage!,
                  style: const TextStyle(color: Colors.red),
                  textAlign: TextAlign.center),
            if (_successMessage != null)
              Text(_successMessage!,
                  style: const TextStyle(color: Colors.green),
                  textAlign: TextAlign.center),
            const SizedBox(height: 20),
            Expanded(child: _buildStepContent()),
          ],
        ),
      ),
    );
  }
}
