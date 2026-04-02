import 'dart:io';
import 'package:flutter/material.dart';
import 'package:frontend_qlte/config/config_url.dart';
import 'package:frontend_qlte/models/user/update_user_info_request_model.dart';
import 'package:frontend_qlte/services/user_service.dart';
import 'package:image_picker/image_picker.dart';

class UpdateInfoUserScreen extends StatefulWidget {
  final String? fullName;
  final String? userName; // đổi từ email sang userName
  final String? avatar;
  final String? userId;
  final String? phoneNumber;
  final String? gioiTinh;
  final String? moTa;

  const UpdateInfoUserScreen({
    super.key,
    this.fullName,
    this.userName, // đổi từ email
    this.avatar,
    this.userId,
    this.phoneNumber,
    this.gioiTinh,
    this.moTa,
  });

  @override
  State<UpdateInfoUserScreen> createState() => _UpdateInfoUserScreenState();
}

class _UpdateInfoUserScreenState extends State<UpdateInfoUserScreen> {
  final TextEditingController _fullNameController = TextEditingController();
  final TextEditingController _userNameController =
      TextEditingController(); // user name
  final TextEditingController _phoneController = TextEditingController();
  final TextEditingController _moTaController = TextEditingController();

  String? _selectedGioiTinh;
  File? _selectedImage;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _fullNameController.text = widget.fullName ?? '';
    _userNameController.text = widget.userName ?? ''; // đổi từ email
    _phoneController.text = widget.phoneNumber ?? '';
    _moTaController.text = widget.moTa ?? '';
    print(widget.moTa);
    _selectedGioiTinh = widget.gioiTinh ?? "Chưa xác định";
  }

  Future<void> _pickImage() async {
    final picked = await ImagePicker().pickImage(source: ImageSource.gallery);
    if (picked != null) {
      setState(() {
        _selectedImage = File(picked.path);
      });
    }
  }

  Future<void> _updateInfoUser() async {
    if (widget.userId == null || widget.userId!.isEmpty) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('UserId không hợp lệ')));
      return;
    }

    setState(() => _isLoading = true);

    try {
      int? gioiTinhInt;
      if (_selectedGioiTinh == "Nam") {
        gioiTinhInt = 1;
      } else if (_selectedGioiTinh == "Nữ") {
        gioiTinhInt = 2;
      } else {
        gioiTinhInt = 0;
      }

      final request = UpdateUserInfoRequestModel(
        userId: widget.userId!, // luôn phải có
        nameND: _fullNameController.text.trim().isNotEmpty
            ? _fullNameController.text.trim()
            : widget.fullName ?? '', // dùng fullName cũ
        userName: _userNameController.text.trim().isNotEmpty
            ? _userNameController.text.trim()
            : widget.userName ?? '', // dùng userName cũ
        phoneNumber: _phoneController.text.trim().isNotEmpty
            ? _phoneController.text.trim()
            : widget.phoneNumber ?? '',
        avatarND: _selectedImage?.path, // để null nếu không thay avatar
        gioiTinh: gioiTinhInt,
        moTa: _moTaController.text.trim().isNotEmpty
            ? _moTaController.text.trim()
            : widget.moTa ?? '',
      );

      final response = await UserService.updateInfoUserAsync(request);
     print(response?.nameND);
      if (response != null) {
          Navigator.pop(context, response); 
      }
    } catch (e) {
      print("❌ Lỗi cập nhật user: $e");
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text('Cập nhật thất bại: $e')));
    } finally {
      setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Cập nhật thông tin cá nhân'),
        centerTitle: true,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: Column(
          children: [
            Center(
              child: Stack(
                children: [
                  CircleAvatar(
                    radius: 55,
                    backgroundColor: Colors.grey[300],
                    backgroundImage: _selectedImage != null
                        ? FileImage(_selectedImage!)
                        : (widget.avatar != null && widget.avatar!.isNotEmpty
                              ? (widget.avatar!.startsWith("http")
                                    ? NetworkImage(widget.avatar!)
                                    : NetworkImage(
                                        "${Config_URL.urlServer}${widget.avatar!}",
                                      ))
                              : const AssetImage(
                                      'assets/images/default_avatar.png',
                                    )
                                    as ImageProvider),
                  ),
                  Positioned(
                    bottom: 0,
                    right: 0,
                    child: InkWell(
                      onTap: _pickImage,
                      child: const CircleAvatar(
                        radius: 18,
                        backgroundColor: Colors.teal,
                        child: Icon(Icons.camera_alt, color: Colors.white),
                      ),
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 30),
            _buildInput(_fullNameController, 'Họ và tên'),
            const SizedBox(height: 16),
            _buildInput(_userNameController, 'Tên đăng nhập'), // đổi label
            const SizedBox(height: 16),
            _buildInput(_phoneController, 'Số điện thoại'),
            const SizedBox(height: 16),
            _buildDropdownGioiTinh(),
            const SizedBox(height: 16),
            _buildInput(_moTaController, 'Mô tả'),
            const SizedBox(height: 30),
            SizedBox(
              width: double.infinity,
              height: 48,
              child: ElevatedButton.icon(
                icon: const Icon(Icons.save),
                label: _isLoading
                    ? const CircularProgressIndicator(
                        color: Colors.white,
                        strokeWidth: 2,
                      )
                    : const Text(
                        'Lưu thay đổi',
                        style: TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.teal,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
                onPressed: _isLoading ? null : _updateInfoUser,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildInput(TextEditingController controller, String hint) {
    return TextField(
      controller: controller,
      decoration: InputDecoration(
        labelText: hint,
        filled: true,
        fillColor: Colors.grey[100],
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: BorderSide.none,
        ),
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 16,
          vertical: 14,
        ),
      ),
    );
  }

  Widget _buildDropdownGioiTinh() {
    return DropdownButtonFormField<String>(
      value: _selectedGioiTinh,
      decoration: InputDecoration(
        labelText: 'Giới tính',
        filled: true,
        fillColor: Colors.grey[100],
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: BorderSide.none,
        ),
      ),
      items: const [
        DropdownMenuItem(value: "Nam", child: Text("Nam")),
        DropdownMenuItem(value: "Nữ", child: Text("Nữ")),
        DropdownMenuItem(value: "Chưa xác định", child: Text("Chưa xác định")),
      ],
      onChanged: (value) {
        setState(() {
          _selectedGioiTinh = value;
        });
      },
    );
  }
}
