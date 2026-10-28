using System;
using System.Data;
using System.Data.SqlClient; // Thư viện quan trọng giúp hết lỗi gạch đỏ cho SqlConnection, SqlDataAdapter
using System.Windows.Forms;

namespace Bai5._1
{
    public partial class Form1 : Form
    {
        // Chuỗi kết nối SQL Server với Server Name của bạn
        private string connectionString = @"Data Source=CHINH\SQLEXPRESS;Initial Catalog=ql_sinhvien;Integrated Security=True";

        public Form1()
        {
            InitializeComponent();

            // Gán phím tắt nhanh (Enter để đăng nhập, Esc để thoát)
            this.AcceptButton = btnLogin;
            this.CancelButton = btnExit;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Mặc định ẩn mật khẩu thành dấu sao
            txtPassword.UseSystemPasswordChar = true;
            chkShowPassword.Checked = false;
        }

        // Xử lý sự kiện Đăng nhập kết hợp Validation và kiểm tra SQL Server
        private void btnLogin_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            bool isValid = true;

            // 1. Kiểm tra Validation dữ liệu trống
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                errorProvider1.SetError(txtUsername, "Vui lòng nhập tên đăng nhập!");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                errorProvider1.SetError(txtPassword, "Vui lòng nhập mật khẩu!");
                isValid = false;
            }

            // Nếu dữ liệu chưa hợp lệ thì dừng lại
            if (!isValid) return;

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            // 2. Kiểm tra tài khoản trong CSDL SQL Server (Chống SQL Injection)
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND Password = @Password";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            // Thông báo thành công kèm tên của bạn
                            MessageBox.Show("Đăng nhập thành công! Chào mừng Phạm Duy Chinh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Gọi hàm tải danh sách sinh viên lên DataGridView từ SQL Server
                            LoadDanhSachSinhVien();
                        }
                        else
                        {
                            MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtPassword.Clear();
                            txtPassword.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối SQL Server: {ex.Message}", "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý ẩn/hiện mật khẩu khi check vào CheckBox
        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        // Hàm tải dữ liệu danh sách sinh viên lên DataGridView
        private void LoadDanhSachSinhVien()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "SELECT MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB FROM SinhVien";
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Gán vào DataGridView trên Form
                    if (dgvSinhVien != null)
                    {
                        dgvSinhVien.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu sinh viên: {ex.Message}", "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý nút Thoát (kích hoạt bằng phím Esc)
        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}