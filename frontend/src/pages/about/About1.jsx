import React from 'react';

export default function About1() {
  return (
    <div className="history-page">
      <header className="page-hero text-center py-5">
        <div className="container">
          <h1
            className="fw-bold"
            style={{ color: 'var(--primary)' }}
          >
            Lịch sử hình thành
          </h1>
        </div>
      </header>

      <main className="main-content bg-white">
        <div className="container py-4">
          <div className="row justify-content-center">
            <div
              className="col-12 content-col"
              style={{ maxWidth: '900px' }}
            >
              {/* 1. Giới thiệu */}
              <section
                aria-labelledby="gioi-thieu"
                className="mb-4"
              >
                <h2
                  id="gioi-thieu"
                  className="fw-bold fs-4 mb-3"
                  style={{ color: 'var(--primary)' }}
                >
                  1. Giới thiệu
                </h2>

                <p
                  className="lead-like"
                  style={{
                    fontSize: '1.05rem',
                    lineHeight: '1.7',
                  }}
                >
                  Phòng khám Xét nghiệm Chuyên sâu Bio Medic Center
                  được thành lập với sứ mệnh mang đến dịch vụ xét
                  nghiệm y tế chính xác – nhanh chóng – cá nhân hóa.
                  Từ một phòng lab nhỏ năm 2010, chúng tôi đã phát
                  triển thành trung tâm hàng đầu tại Việt Nam, chuyên
                  sâu về bệnh lý thận–tiết niệu, nhiễm virus–vi khuẩn
                  và các xét nghiệm cao cấp. Với đội ngũ bác sĩ giàu
                  kinh nghiệm và công nghệ hiện đại, Bio Medic Center
                  cam kết đồng hành cùng bạn trong hành trình chăm sóc
                  sức khỏe.
                </p>
              </section>

              {/* 2. Lịch sử hình thành */}
              <section
                aria-labelledby="lich-su"
                className="mb-4"
              >
                <h2
                  id="lich-su"
                  className="fw-bold fs-4 mb-3"
                  style={{ color: 'var(--primary)' }}
                >
                  2. Lịch sử hình thành
                </h2>

                <div className="timeline">
                  <p style={{ lineHeight: '1.7' }}>
                    <b style={{ color: 'var(--primary)' }}>
                      2010: Thành lập nền tảng.{' '}
                    </b>

                    Bio Medic Center được sáng lập bởi nhóm bác sĩ
                    chuyên khoa xét nghiệm từ Bệnh viện Bạch Mai và
                    Bệnh viện T.Ư Quân đội 108, cung cấp dịch vụ xét
                    nghiệm cơ bản tại TP. Hà Nội. Giai đoạn đầu tập
                    trung xét nghiệm thận–tiết niệu (ure, creatinin
                    niệu, tổng phân tích nước tiểu – TPU) và một số xét
                    nghiệm nhiễm virus–vi khuẩn (cấy máu/dịch, RPR).
                    Với diện tích ~200&nbsp;m² và 5 nhân sự, chúng tôi
                    nhanh chóng nhận được sự tin tưởng nhờ độ chính xác
                    và chi phí hợp lý.
                  </p>

                  <p style={{ lineHeight: '1.7' }}>
                    <b style={{ color: 'var(--primary)' }}>
                      <time dateTime="2015">2015</time>: Mở rộng chuyên
                      môn và công nghệ.{' '}
                    </b>{' '}

                    Đáp ứng nhu cầu tăng ở nhóm bệnh nhân đái tháo
                    đường và tăng huyết áp, trung tâm đầu tư máy sinh
                    hóa tự động (Roche Cobas), bổ sung chỉ số thận như
                    microalbumin niệu (MAU), cystatin C và điện giải
                    đồ. Đồng thời triển khai gói xét nghiệm nhiễm
                    virus–vi khuẩn cho các bệnh phổ biến (ví dụ:
                    Dengue NS1 + IgM/IgG, sốt rét). Diện tích mở rộng
                    lên ~500&nbsp;m²; hợp tác đào tạo với Viện Pasteur;
                    đạt chứng nhận ISO 15189.
                  </p>

                  <p style={{ lineHeight: '1.7' }}>
                    <b style={{ color: 'var(--primary)' }}>
                      <time dateTime="2018">2018</time>: Bước ngoặt với
                      xét nghiệm cao cấp.{' '}
                    </b>{' '}

                    Phát triển mạnh mẽ mảng di truyền: NIPT (sàng lọc
                    trước sinh không xâm lấn), tầm soát gen
                    Thalassemia, xét nghiệm huyết thống, và HLA typing
                    cho ghép tạng. Hợp tác quốc tế với Illumina để ứng
                    dụng Next-Generation Sequencing (NGS). Phục vụ
                    &gt;5.000 bệnh nhân/năm; được ghi nhận là đơn vị
                    xét nghiệm tiêu biểu.
                  </p>

                  <p style={{ lineHeight: '1.7' }}>
                    <b style={{ color: 'var(--primary)' }}>
                      <time dateTime="2020">2020–2022</time>: Vượt qua
                      đại dịch, số hóa dịch vụ.{' '}
                    </b>{' '}

                    Trong đại dịch COVID-19, trung tâm mở rộng xét
                    nghiệm virus (PCR cúm A, xét nghiệm
                    Varicella-Zoster – VZV IgM/IgG, sởi/quai bị…),
                    triển khai lấy mẫu tại nhà và trả kết quả trực
                    tuyến qua ứng dụng. Các gói sàng lọc thận được tối
                    ưu cho bệnh nhân mạn tính. Duy trì tăng trưởng
                    doanh thu ~30%, và đạt chứng nhận CAP (College of
                    American Pathologists) cho mảng lab cao cấp.
                  </p>

                  <p style={{ lineHeight: '1.7' }}>
                    <b style={{ color: 'var(--primary)' }}>
                      <time dateTime="2023">2023–2025</time>: Phát
                      triển bền vững và mở rộng.{' '}
                    </b>{' '}

                    Đến 2025, Bio Medic Center vận hành 3 chi nhánh tại
                    Hà Nội, TP. HCM và Đà Nẵng, phục vụ &gt;20.000
                    bệnh nhân/năm. Hoàn thiện danh mục: sàng lọc gen
                    ung thư, karyotype, và xét nghiệm kháng thể thụ
                    thể acetylcholine (AChR) cho bệnh tự miễn. Ứng dụng
                    AI hỗ trợ phân tích (nâng hiệu quả và độ nhất quán
                    báo cáo), hợp tác chuyên môn với các bệnh viện lớn.
                    Định hướng tiếp theo: mở rộng PGT (Preimplantation
                    Genetic Testing) và các dịch vụ y học cá nhân hóa.
                  </p>
                </div>
              </section>

              {/* 3. Lời kết */}
              <section
                aria-labelledby="loi-ket"
                className="mb-2"
              >
                <h2
                  id="loi-ket"
                  className="fw-bold fs-4 mb-3"
                  style={{ color: 'var(--primary)' }}
                >
                  3. Lời kết
                </h2>

                <p
                  className="lead-like"
                  style={{
                    fontSize: '1.05rem',
                    lineHeight: '1.7',
                  }}
                >
                  Hành trình của Bio Medic Center là sự kết hợp giữa
                  chất lượng chuyên môn, đầu tư công nghệ và trải
                  nghiệm bệnh nhân. Chúng tôi tự hào là đối tác đáng
                  tin cậy trong chẩn đoán và phòng ngừa bệnh tật. Hãy
                  liên hệ để trải nghiệm dịch vụ xét nghiệm chuyên sâu
                  ngay hôm nay!
                </p>
              </section>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}