import React from 'react';

export default function NewsXetNghiem() {
  return (
    <div
      className="home-test-promotion-page py-4"
      style={{
        fontFamily:
          'Arial, sans-serif',

        lineHeight:
          '1.6',

        background:
          '#f4f6f8',

        minHeight:
          '100vh',
      }}
    >
      <div
        className="container"
        style={{
          maxWidth:
            '850px',

          margin:
            '30px auto',

          background:
            '#fff',

          padding:
            '30px',

          borderRadius:
            '10px',

          boxShadow:
            '0 4px 12px rgba(0, 0, 0, 0.1)',
        }}
      >
        <h1
          className="fw-bold"
          style={{
            color:
              '#28a745',

            marginBottom:
              '15px',
          }}
        >
          Tiết kiệm ngay 500K khi xét nghiệm tại nhà
        </h1>

        <div
          className="date text-muted mb-3"
          style={{
            fontSize:
              '14px',
          }}
        >
          Chương trình áp dụng theo thời gian ưu đãi
          của Bio Medic Center
        </div>

        <p
          style={{
            marginBottom:
              '15px',

            textAlign:
              'justify',
          }}
        >
          Trong thời gian chương trình diễn ra, Bio
          Medic Center triển khai ưu đãi đặc biệt cho
          khách hàng:{' '}

          <span
            className="fw-bold"
            style={{
              color:
                '#c9302c',
            }}
          >
            giảm ngay 500,000đ
          </span>{' '}

          khi đăng ký gói xét nghiệm tại nhà đủ điều
          kiện.
        </p>

        <p
          style={{
            marginBottom:
              '15px',

            textAlign:
              'justify',
          }}
        >
          Dịch vụ lấy mẫu tận nơi mang lại sự thuận
          tiện, phù hợp với người cao tuổi, trẻ nhỏ,
          người khó di chuyển hoặc khách hàng bận
          rộn.
        </p>

        <p
          style={{
            marginBottom:
              '15px',

            textAlign:
              'justify',
          }}
        >
          Bên cạnh ưu đãi trên, khách hàng có thể
          được hưởng thêm các chương trình khuyến mãi
          khác theo chính sách đang áp dụng tại Bio
          Medic Center.
        </p>

        <p
          style={{
            marginBottom:
              0,

            textAlign:
              'justify',
          }}
        >
          Đặt lịch để chủ động chăm sóc sức khỏe cho
          bạn và gia đình một cách thuận tiện và hiệu
          quả.
        </p>
      </div>
    </div>
  );
}