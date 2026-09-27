import React from 'react';

export default function NewsTimMach() {
  return (
    <div
      className="promotion-page py-4"
      style={{
        fontFamily:
          'Verdana, sans-serif',

        background:
          '#eef2f5',

        minHeight:
          '100vh',
      }}
    >
      <div
        className="container"
        style={{
          maxWidth:
            '880px',

          margin:
            '30px auto',

          background:
            '#fff',

          padding:
            '35px',

          borderRadius:
            '10px',

          boxShadow:
            '0 4px 14px rgba(0, 0, 0, 0.1)',
        }}
      >
        <h1
          className="fw-bold"
          style={{
            color:
              '#b30059',

            marginBottom:
              '20px',
          }}
        >
          Giảm 40% gói khám tim mạch
        </h1>

        <p
          style={{
            marginBottom:
              '18px',

            textAlign:
              'justify',

            lineHeight:
              '1.6',
          }}
        >
          Với mong muốn nâng cao sức khỏe cộng đồng
          và phòng ngừa sớm các bệnh lý tim mạch,
          Bio Medic Center triển khai chương trình{' '}

          <span
            className="fw-bold"
            style={{
              color:
                '#d9534f',
            }}
          >
            giảm 40% chi phí gói khám tim mạch toàn
            diện
          </span>
          .
        </p>

        <p
          style={{
            marginBottom:
              '10px',

            textAlign:
              'justify',

            lineHeight:
              '1.6',
          }}
        >
          Gói khám tim mạch bao gồm:
        </p>

        <ul
          style={{
            margin:
              '10px 0 20px 20px',

            lineHeight:
              '1.8',
          }}
        >
          <li>
            Siêu âm tim
          </li>

          <li>
            Điện tâm đồ (ECG)
          </li>

          <li>
            Xét nghiệm máu chuyên sâu
          </li>

          <li>
            Tư vấn và chẩn đoán trực tiếp từ bác sĩ
            chuyên khoa
          </li>
        </ul>

        <p
          style={{
            marginBottom:
              '18px',

            textAlign:
              'justify',

            lineHeight:
              '1.6',
          }}
        >
          Bệnh lý tim mạch là một trong những vấn đề
          sức khỏe cần được theo dõi và phát hiện
          sớm. Kiểm tra định kỳ giúp đánh giá nguy
          cơ, hỗ trợ chẩn đoán và xây dựng kế hoạch
          chăm sóc phù hợp.
        </p>

        <p
          style={{
            marginBottom:
              0,

            textAlign:
              'justify',

            lineHeight:
              '1.6',
          }}
        >
          Hãy đăng ký để tận dụng ưu đãi và chủ động
          chăm sóc sức khỏe tim mạch cho bạn và gia
          đình.
        </p>
      </div>
    </div>
  );
}